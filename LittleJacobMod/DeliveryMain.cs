using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using LittleJacobMod.Interface;

class DeliveryMain : Script
{
    struct HeadingPoint
    {
        public float X;
        public float Y;
        public float Z;
        public float Heading;
        public int Hash;
    };

    public static bool Active { get; private set; }
    public static event EventHandler OnDeliveryCompleted;
    int _startTime;
    RelationshipGroup _neutral;
    RelationshipGroup _hateCops;
    int _cop;
    Blip _routeBlip;
    Ped _buyer;
    Vector3 _destination;
    Vehicle _car;
    int _objective;
    int _blipST;
    int _pChanceL = 8;
    int _pChanceH = 30;
    Prop _bag;
    bool _htShown;
    bool _bagTaken;
    bool _pigFlag;
    int _lockedAt;
    bool _carRecovered;
    bool _fighting;
    int _travelTime;
    int _intensity;
    int _travelStartTime = 1;
    int _health;
    readonly Vector3 _dropPoint = new Vector3(6.828116f, -1405.562f, 28.26828f);
    Dictionary<int, Vector3> _badgeSlots = new Dictionary<int, Vector3>()
    {
        {0, new Vector3(0.91f, 0.97f, 0)},
        {1, new Vector3(0.91f, 0.918f, 0)}
    };

    Dictionary<int, Vector3> _contentSlots = new Dictionary<int, Vector3>()
    {
        {0, new Vector3(0.75f, 0.952f, 0)},
        {1, new Vector3(0.75f, 0.90f, 0)}
    };

    Dictionary<int, Vector3> _titleSlots = new Dictionary<int, Vector3>()
    {
        {0, new Vector3(0.855f, 0.96f, 0)},
        {1, new Vector3(0.855f, 0.91f, 0)}
    };

    public DeliveryMain()
    {
        _neutral = World.AddRelationshipGroup("DRUG_DELIVERY_NEUTRAL_REL");
        _hateCops = World.AddRelationshipGroup("DRUG_DELIVERY_HATE_COPS_REL");
        _cop = Game.GenerateHash("COP");
        int player = Game.GenerateHash("PLAYER");
        _neutral.SetRelationshipBetweenGroups(player, Relationship.Neutral, true);
        _neutral.SetRelationshipBetweenGroups(_cop, Relationship.Neutral, true);
        _hateCops.SetRelationshipBetweenGroups(player, Relationship.Like, true);
        _hateCops.SetRelationshipBetweenGroups(_cop, Relationship.Hate, true);
        CallMenu.DeliverySelected += Start;
        Aborted += DeliveryMain_Aborted;
    }

    void LoadSprites()
    {
        Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, "timerbars", false);

        while (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, "timerbars"))
            Wait(1);
    }

    void UnloadSprites()
    {
        Function.Call(Hash.SET_STREAMED_TEXTURE_DICT_AS_NO_LONGER_NEEDED, "timerbars");
    }

    void DrawSprite(float x, float y, bool red)
    {
        if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, "timerbars"))
            LoadSprites();

        Function.Call(Hash.DRAW_SPRITE, "timerbars", "all_black_bg", x, y, 0.15f, 0.045f, 0, 0, 0, 0, 200);

        if (red)
            Function.Call(Hash.DRAW_SPRITE, "timerbars", "all_red_bg", x, y, 0.15f, 0.045f, 0, 165, 15, 1, 255);
    }

    void DrawSpriteText(float x, float y, float scale, string text, bool right)
    {
    
        Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
    	Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME,  text);
        
    	if (right)
    	{
    		Function.Call(Hash.SET_TEXT_WRAP, 0.6f, 0.975f);
    		Function.Call(Hash.SET_TEXT_JUSTIFICATION, 2);
    	}

        Function.Call(Hash.SET_TEXT_SCALE, 1.0f, scale);
        Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, x, y);
    }

    void DrawBadge(string title, string content, bool red, int slot)
    {
    
        Vector3 bPos = _badgeSlots[slot];
        Vector3 cPos = _contentSlots[slot];
        Vector3 tPos = _titleSlots[slot];
        DrawSprite(bPos.X, bPos.Y, red);
        DrawSpriteText(cPos.X, cPos.Y, 0.42f, content, true);
        DrawSpriteText(tPos.X, tPos.Y, 0.295f, title, false);
    }

private void DeliveryMain_Aborted(object sender, EventArgs e)
    {
        if (Active)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(5));
            ToggleMusicInterrup(false);
            UnloadSprites();
            Clean();
        }
    }

    void RequestModel(int model)
    {
        Function.Call(Hash.REQUEST_MODEL, model);

        while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, model))
            Wait(1);
    }

    void RequestModel(uint model)
    {
        Function.Call(Hash.REQUEST_MODEL, model);

        while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, model))
            Wait(1);
    }

    string GetEvent(int param)
    {
        switch (param)
        {
            case -1:
                return "VWC_IDLE";
            case 0:
                return "VWC_IDLE_START";
            case 1:
                return "VWC_MED_INTENSITY";
            case 2:
                return "VWC_GUNFIGHT";
            case 3:
                return "VWC_VEHICLE_ACTION";
            case 4:
                return "MP_DM_COUNTDOWN_KILL";
            case 5:
                return "VWC_FAIL";
            default:
                return "";
        }
    }

    void Start(object o, EventArgs e)
    {
        XElement doc = XElement.Load($"{BaseDirectory}\\LittleJacobMod\\Missions\\drug_delivery.xml");
        IEnumerable<XElement> cars = doc.Element("CarLocations").Descendants("Location");
        int size = cars.Count();
        List<HeadingPoint> points = new List<HeadingPoint>();
        Random ran = new Random();

        for (int i = 0; i < size; i++)
        {
            HeadingPoint point = new HeadingPoint();
            XElement el = cars.ElementAt(i);
            point.X = (float)el.Element("X");
            point.Y = (float)el.Element("Y");
            point.Z = (float)el.Element("Z");
            point.Heading = (float)el.Element("Heading");

            if (Game.Player.Character.IsInRange(new Vector3(point.X, point.Y, point.Z), 70))
                continue;

            points.Add(point);
        }

        HeadingPoint carPoint = points.ElementAt(ran.Next(0, points.Count));
        points.Clear();
        IEnumerable<XElement> buyers = doc.Elements("BuyerLocations").Descendants("Location");
        size = buyers.Count();
        List<HeadingPoint> validBuyers = new List<HeadingPoint>();
        List<Vector3> markers = new List<Vector3>();
        Vector3 carVector = new Vector3(carPoint.X, carPoint.Y, carPoint.Z);

        for (int i = 0; i < size; i++)
        {
            XElement el = buyers.ElementAt(i);
            XElement buyer = el.Element("Buyer");
            XElement marker = el.Element("Marker");
            Vector3 markerPoint = new Vector3();
            markerPoint.X = (float)marker.Element("X");
            markerPoint.Y = (float)marker.Element("Y");
            markerPoint.Z = (float)marker.Element("Z");
            float dist = World.CalculateTravelDistance(carVector, markerPoint);

            if (dist < 1500)
                continue;

            HeadingPoint buyerPoint = new HeadingPoint();
            buyerPoint.X = (float)buyer.Element("X");
            buyerPoint.Y = (float)buyer.Element("Y");
            buyerPoint.Z = (float)buyer.Element("Z");
            buyerPoint.Heading = (float)buyer.Element("Heading");
            buyerPoint.Hash = (int)buyer.Element("Hash");
            validBuyers.Add(buyerPoint);
            markers.Add(markerPoint);
        }
        
        if (validBuyers.Count == 0)
        {
            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Delivery", "Sorry man. I don't have any deliveries to make.");
        } else
        {
            int index = ran.Next(0, validBuyers.Count);
            HeadingPoint selectedBuyer = validBuyers.ElementAt(index);
            RequestModel(selectedBuyer.Hash);
            _buyer = Function.Call<Ped>(Hash.CREATE_PED, 0, selectedBuyer.Hash, selectedBuyer.X, selectedBuyer.Y,
                selectedBuyer.Z, selectedBuyer.Heading, false, false);
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, selectedBuyer.Hash);
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, _buyer.Handle, true);
            _buyer.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
            _buyer.RelationshipGroup = _neutral;
            _buyer.Weapons.Give(WeaponHash.Pistol, 100, false, true);
            Vector3 bagLocation = _buyer.Position.Around(1.5f);
            RequestModel(3898412430);
            _bag = Function.Call<Prop>(Hash.CREATE_OBJECT, 3898412430, bagLocation.X, bagLocation.Y,
                bagLocation.Z, false, false, false);
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, 3898412430);
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, _bag.Handle, true);
            Function.Call(Hash.PLACE_OBJECT_ON_GROUND_PROPERLY, _bag.Handle);
            _destination = markers.ElementAt(index);
            RequestModel((uint)VehicleHash.Tornado3);
            _car = Function.Call<Vehicle>(Hash.CREATE_VEHICLE, VehicleHash.Tornado3, carPoint.X, carPoint.Y,
                carPoint.Z, carPoint.Heading, false, false);
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, VehicleHash.Tornado3);
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, _car.Handle, true);
            CreateCarBlip();
            Main.ShowScaleform("~g~Weed Delivery", "", 0);
            ToggleMusicInterrup(true);
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(0));
            _health = _car.Health;
            _fighting = false;
            _lockedAt = 0;
            _travelStartTime = 1;
            _bagTaken = false;
            _carRecovered = false;
            _objective = -1;
            Tick += DeliveryMain_Tick;
            Active = true;
        }
    }

    void CreateDestinationBlip(int type)
    {
        _routeBlip = World.CreateBlip(_destination);
        _routeBlip.Scale = 0.8f;
        _routeBlip.Color = BlipColor.Yellow;
        _routeBlip.Name = type == 0 ? "Buyer" : "Drop point";
        _routeBlip.ShowRoute = true;
        _blipST = Game.GameTime;
    }

    void CreateCarBlip()
    {
        _car.AddBlip();
        _car.AttachedBlip.Scale = 0.8f;
        _car.AttachedBlip.Sprite = BlipSprite.Weed;
        _car.AttachedBlip.Color = BlipColor.Green;
        _car.AttachedBlip.Name = "Jacob's car";
    }

    void DEL_0()
    {
        if (_buyer.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Buyer is dead", 0);
            Quit();
            return;
        }
        else if (_car.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Car Destroyed", 0);
            Quit();
            return;
        } else if (Game.Player.WantedLevel > 0 && !_pigFlag)
        {
            _pigFlag = true;
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            _car.AttachedBlip.Delete();
            return;
        } else if (Game.Player.WantedLevel > 0 && _pigFlag)
        {
            GTA.UI.Screen.ShowSubtitle("Lose the cops");
            return;
        } else if (Game.Player.WantedLevel == 0 && _pigFlag)
        {
            _pigFlag = false;
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(-1));
            CreateCarBlip();
        }

        GTA.UI.Screen.ShowSubtitle("Get in the ~g~car.", 1000);

        if (!Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Main.PPID, _car.Handle, false))
            return;

        _car.AttachedBlip.Delete();
        Random ran = new Random();

        if (ran.Next(1, 101) <= _pChanceL)
        {
            GTA.UI.Screen.ShowHelpText("The police were watching this car. Lose them!", 8000);
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            float distance = World.CalculateTravelDistance(Game.Player.Character.Position, _destination);
            _travelTime = (int)Math.Ceiling(distance * 60);

            if (_travelTime > 330000)
                _travelTime = 330000;

            _travelStartTime = Game.GameTime;
            _objective = 2;
        } else
        {
            CreateDestinationBlip(0);
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(1));
            float distance = World.CalculateTravelDistance(Game.Player.Character.Position, _destination);
            _travelTime = (int)Math.Ceiling(distance * 60);

            if (_travelTime > 330000)
                _travelTime = 330000;

            _travelStartTime = Game.GameTime;
            _objective = 1;
        }
    }

    void DEL_1()
    {
        if (_buyer.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Buyer is dead", 0);
            Quit();
            return;
        }
        else if (_car.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Car Destroyed", 0);
            Quit();
            return;
        } else if (Game.Player.WantedLevel > 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            _routeBlip.Delete();
            _objective = 2;
            return;
        }

        GTA.UI.Screen.ShowSubtitle($"Go to the ~y~buyer.", 1000);
        Function.Call(Hash.DRAW_MARKER, 1, _destination.X, _destination.Y, _destination.Z, 0, 0, 0, 0, 0,
                    0, 5f, 5f, 2f, 255, 255, 0, 100, false, false, 2, false, false, false);

        if (_car.IsInRange(_destination, 5))
        {
            _routeBlip.Delete();
            Random ran = new Random();
            
            if (ran.Next(0, 101) <= _pChanceL)
            {
                if (ran.Next(0, 101) <= _pChanceH)
                {
                    GTA.UI.Screen.ShowHelpText("The buyer is a cop. Get out of there!", 8000);
                    Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, _buyer.Handle, _cop);
                    Function.Call(Hash.SET_PED_AS_COP, _buyer.Handle, true);
                    _buyer.Task.FightAgainst(Game.Player.Character);
                    Game.Player.WantedLevel = 4;
                } else
                {
                    GTA.UI.Screen.ShowHelpText("The cops were alerted of the deal. Lose them!", 8000);
                    _buyer.RelationshipGroup = _hateCops;
                    _buyer.Task.FightAgainstHatedTargets(100);
                    Game.Player.WantedLevel = 3;
                }
                _objective = 5;
                _destination = _dropPoint;
            } else
            {
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(-1));
                Game.Player.Character.Task.LeaveVehicle();
                _car.Velocity = Vector3.Zero;
                _car.LockStatus = VehicleLockStatus.PlayerCannotEnter;
                _bag.AddBlip();
                _bag.AttachedBlip.Scale = 0.75f;
                _bag.AttachedBlip.Color = BlipColor.Green;
                _bag.AttachedBlip.Name = "Money";
                Wait(1000);
                TaskSequence sequence = new TaskSequence();
                sequence.AddTask.ClearAllImmediately();
                sequence.AddTask.EnterVehicle(_car, VehicleSeat.Driver);
                sequence.AddTask.CruiseWithVehicle(_car, 50, DrivingStyle.Rushed);
                _buyer.Task.PerformSequence(sequence);
                _objective = 4;
            }

            return;
        } else if (!Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Main.PPID, _car.Handle, false))
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(-1));
            _routeBlip.Delete();
            CreateCarBlip();
            _objective = 3;
        }
    }

    void DEL_2()
    {
        if (_buyer.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Buyer is dead", 0);
            Quit();
            return;
        }
        else if (_car.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Car Destroyed", 0);
            Quit();
            return;
        }

        GTA.UI.Screen.ShowSubtitle("Lose the cops.", 1000);

        if (Game.Player.WantedLevel == 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(_intensity));
            CreateCarBlip();
            _objective = 3;
            return;
        }

        if (Game.Player.Character.IsInRange(_destination, 50))
        {
            if (Game.Player.Character.IsInRange(_destination, 20))
            {
                Random ran = new Random();

                if (ran.Next(0, 101) <= _pChanceH)
                {
                    GTA.UI.Screen.ShowHelpText("The buyer is a cop. Get out of there!", 8000);
                    Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, _buyer.Handle, _cop);
                    Function.Call(Hash.SET_PED_AS_COP, _buyer.Handle, true);
                    _buyer.Task.FightAgainst(Game.Player.Character);
                    Game.Player.WantedLevel = 4;
                    _objective = 5;
                    _destination = _dropPoint;
                } else
                {
                    if (!Function.Call<bool>(Hash.ARE_PLAYER_STARS_GREYED_OUT, Game.Player.Handle))
                    {
                        _buyer.Task.FleeFrom(_destination);
                        _destination = _dropPoint;
                        GTA.UI.Screen.ShowHelpText("Deal canceled. Buyer was scared.", 8000);
                        _objective = 5;
                    }
                    else
                    {
                        Game.Player.Character.Task.LeaveVehicle();
                        _car.Velocity = Vector3.Zero;
                        _car.LockStatus = VehicleLockStatus.PlayerCannotEnter;
                        _bag.AddBlip();
                        _bag.AttachedBlip.Scale = 0.75f;
                        _bag.AttachedBlip.Color = BlipColor.Green;
                        _bag.AttachedBlip.Name = "Money";
                        Wait(1000);
                        TaskSequence sequence = new TaskSequence();
                        sequence.AddTask.ClearAllImmediately();
                        sequence.AddTask.CruiseWithVehicle(_car, 50, DrivingStyle.Rushed);
                        _buyer.Task.PerformSequence(sequence);
                        _objective = 4;
                    }
                }
            }
        }
    }

    void DEL_3()
    {
        if (_buyer.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Buyer is dead", 0);
            Quit();
            return;
        }
        else if (_car.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Car Destroyed", 0);
            Quit();
            return;
        } else if (Game.Player.WantedLevel > 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            _car.AttachedBlip.Delete();
            _objective = 2;
            return;
        }

        GTA.UI.Screen.ShowSubtitle("Go back to the ~g~car.", 1000);

        if (!Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Main.PPID, _car.Handle, false))
            return;

        if (_car.AttachedBlip != null)
        {
            _car.AttachedBlip.Delete();
        }

        CreateDestinationBlip(0);
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(_intensity));
        _objective = 1;
    }

    void DEL_4()
    {
        GTA.UI.Screen.ShowSubtitle("Take the ~g~money~w~ and leave the area.", 1000);

        bool buyerLeft = !_buyer.IsInRange(_destination, 50) || !_buyer.IsInRange(Game.Player.Character.Position, 50);

        if (_bagTaken && buyerLeft)
        {
            Clean();
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(2));
            _destination = _dropPoint;
            CreateDestinationBlip(1);
            _objective = 8;
        } else if (_bagTaken && !buyerLeft && _buyer.IsDead)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            _car.LockStatus = VehicleLockStatus.None;
            _destination = _dropPoint;
            _carRecovered = true;
            _objective = 6;
        }

        if (!_fighting)
        {
            if (Game.Player.Character.IsTryingToEnterALockedVehicle && Game.Player.Character.VehicleTryingToEnter == _car)
            {
                _fighting = true;
                _buyer.Task.ClearAllImmediately();
                _buyer.Task.FightAgainst(Game.Player.Character);
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(2));
            }
        }
    }

    void DEL_5()
    {
        if (_car.IsDead)
        {
            if (_bagTaken)
            {
                _carRecovered = false;
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(2));
                _objective = 8;
            }
            else
            {
                Main.ShowScaleform("~r~Mission failed", "Car Destroyed", 0);
                Quit();
            }

            return;
        }

        GTA.UI.Screen.ShowSubtitle("Lose the cops.", 1000);

        if (Game.Player.WantedLevel == 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(1));
            CreateCarBlip();
            _objective = 6;
        }
    }

    void DEL_6()
    {
        if (_car.IsDead)
        {
            if (_bagTaken)
            {
                _carRecovered = false;
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(2));
                _objective = 8;
            }
            else
            {
                Main.ShowScaleform("~r~Mission failed", "Car Destroyed", 0);
                Quit();
            }

            return;
        } else if (Game.Player.WantedLevel > 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            _objective = 5;
            return;
        }

        GTA.UI.Screen.ShowSubtitle("Go back to the ~g~car.", 1000);

        if (!Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Main.PPID, _car.Handle, false))
            return;

        if (_car.AttachedBlip != null)
        {
            _car.AttachedBlip.Delete();
        }

        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(_bagTaken ? 2 : 1));
        CreateDestinationBlip(1);
        _objective = 7;
    }

    void DEL_7()
    {
        if (_car.IsDead)
        {
            if (_bagTaken)
            {
                _carRecovered = false;
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(2));
                _objective = 8;
            }
            else
            {
                Main.ShowScaleform("~r~Mission failed", "Car Destroyed", 0);
                Quit();
            }

            return;
        }
        else if (Game.Player.WantedLevel > 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            _routeBlip.Delete();
            _objective = 5;
            return;
        }

        GTA.UI.Screen.ShowSubtitle("Take the car to the ~y~drop point.", 1000);
        Function.Call(Hash.DRAW_MARKER, 1, _destination.X, _destination.Y, _destination.Z, 0, 0, 0, 0, 0,
                            0, 5f, 5f, 2f, 255, 255, 0, 100, false, false, 2, false, false, false);

        if (_car.IsInRange(_destination, 5))
        {
            _car.Velocity = Vector3.Zero;
            Game.Player.Character.Task.LeaveVehicle();
            _car.LockStatus = VehicleLockStatus.PlayerCannotEnter;
            Complete(0);
        } else if (!Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Main.PPID, _car.Handle, false))
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(-1));
            _routeBlip.Delete();
            CreateCarBlip();
            _objective = 6;
        }
    }

    void DEL_8()
    {
        if (Game.Player.WantedLevel > 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            _routeBlip.Delete();
            _objective = 9;
            return;
        }

        GTA.UI.Screen.ShowSubtitle("Deliver the money to Jacob.");
        Function.Call(Hash.DRAW_MARKER, 1, _destination.X, _destination.Y, _destination.Z, 0, 0, 0, 0, 0,
                           0, 5f, 5f, 2f, 255, 255, 0, 100, false, false, 2, false, false, false);

        if (Game.Player.Character.IsInRange(_destination, 5) && !Game.Player.Character.IsInVehicle())
            Complete(0);
    }

    void DEL_9()
    {
        GTA.UI.Screen.ShowSubtitle("Lose the cops.");

        if (Game.Player.WantedLevel == 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(2));
            CreateDestinationBlip(1);
            _objective = 8;
        }
    }

    void ToggleMusicInterrup(bool value)
    {
        Function.Call(Hash.SET_AUDIO_FLAG, "DisableFlightMusic", value);
        Function.Call(Hash.SET_AUDIO_FLAG, "WantedMusicDisabled", value);
    }

    void Complete(int bonus)
    {
        OnDeliveryCompleted?.Invoke(this, EventArgs.Empty);

        if (_bagTaken && _carRecovered)
        {
            float multiplier = _health / 10 * 0.01f;
            int carBonus = (int)(80000 * multiplier);
            Game.Player.Money += _lockedAt + carBonus;
        } else
        {
            Game.Player.Money += _lockedAt;
        }

        Main.ShowScaleform("~g~Delivery Completed", "", 0);
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(4));
        ToggleMusicInterrup(false);
        Clean();
        UnloadSprites();
        Active = false;
        Tick -= DeliveryMain_Tick;
    }

    void Quit()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(5));
        ToggleMusicInterrup(false);
        Clean();
        UnloadSprites();
        Active = false;
        Tick -= DeliveryMain_Tick;
    }

    void Clean()
    {
        if (_car != null && _car.Handle != 0)
            _car.MarkAsNoLongerNeeded();

        if (_buyer != null && _buyer.Handle != 0)
            _buyer.MarkAsNoLongerNeeded();

        if (_routeBlip != null && _routeBlip.Handle != 0)
            _routeBlip.Delete();

        if (_bag != null && _bag.Handle != 0)
            _bag.Delete();
    }
    
    string ColorModifier(int value, int baseVal)
    {
        if (value >= baseVal / 3 * 2)
            return "~g~";
        else if (value >= baseVal / 3)
            return "~y~";
        else
            return "~r~";
    }

    private void DeliveryMain_Tick(object sender, EventArgs e)
    {
        if (!Active)
            return;

        if (!Main.ScaleformActive && _objective == -1)
        {
            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Delivery", "Ok, rasta. Just get the ~g~car~w~ and deliver it to the ~y~buyer~w~. The ~y~buyer~w~ is waiting so you better hurry.");
            _objective = 0;
            return;
        }

        if (Game.Player.IsDead)
        {
            Quit();
            return;
        }

        if (_routeBlip != null && _routeBlip.Handle != 0)
        {
            if (Game.GameTime - _blipST >= 2000)
            {
                _routeBlip.ShowRoute = true;
                _blipST = Game.GameTime;
            }
        }

        if (_car.Health < _health)
            _health = _car.Health;

        if (_objective > 0)
        {
            string mod = ColorModifier(_lockedAt, _carRecovered ? 160000 : 80000);

            if (!_bagTaken)
            {
                float multiplier = _health / 10 * 0.01f;
                _lockedAt = (int)(80000 * multiplier);

                if (Game.Player.Character.IsInRange(_bag.Position, 1.25f) && !Game.Player.Character.IsInVehicle())
                {
                    GTA.UI.Screen.ShowHelpTextThisFrame("Press ~INPUT_CONTEXT~ to pick up the ~g~money.", false);

                    if (Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, 51))
                    {
                        Game.Player.Character.Task.PlayAnimation("pickup_object", "pickup_low");
                        Wait(1000);
                        _bag.Delete();
                        _bagTaken = true;

                        if (_objective > 4 && !_carRecovered && !_car.IsDead)
                            _carRecovered = true;
                    }
                }

                DrawBadge($"{mod}REWARD", $"{mod}${_lockedAt}", mod == "~r~", 0);
            } else
            {
                if (_carRecovered)
                {
                    float multiplier = _health / 10 * 0.01f;
                    int carBonus = (int)(80000 * multiplier);
                    mod = ColorModifier(_lockedAt + carBonus, 160000);
                    DrawBadge($"{mod}REWARD", $"{mod}${_lockedAt + carBonus}", mod == "~r~", 0);
                } else
                {
                    DrawBadge($"{mod}REWARD", $"{mod}${_lockedAt}", mod == "~r~", 0);
                }
            }

            if (_objective < 4)
            {
                int remaining = _travelTime - (Game.GameTime - _travelStartTime);
                
                if (Game.GameTime - _travelStartTime >= _travelTime)
                {
                    Main.ShowScaleform("~r~Delivery Failed", "The buyer left.", 0);
                    Quit();
                    return;
                }

                string colorModifier;
                bool red = false;

                if (remaining > _travelTime / 1.5f)
                {
                    colorModifier = "~g~";
                    _intensity = 1;
                }
                else if (remaining > _travelTime / 3)
                {
                    colorModifier = "~y~";

                    if (_intensity == 1 && _objective == 1)
                        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(2));

                    _intensity = 2;
                }
                else
                {
                    colorModifier = "~r~";

                    if (_intensity == 2 && _objective == 1)
                        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));

                    _intensity = 3;
                    red = true;
                }

                string title = $"{colorModifier}TIME LEFT";
                string content = $"{colorModifier}";
                int val = remaining / 1000 / 60;

                if (val < 10)
                    content = string.Concat(content, "0");

                content = string.Concat(content, $"{val}:");
                val = remaining / 1000 % 60;

                if (val < 10)
                    content = string.Concat(content, "0");

                content = string.Concat(content, val);
                DrawBadge(title, content, red, 1);
            }
        }

        switch (_objective)
        {
            case 0:
                DEL_0();
                break;
            case 1:
                DEL_1();
                break;
            case 2:
                DEL_2();
                break;
            case 3:
                DEL_3();
                break;
            case 4:
                DEL_4();
                break;
            case 5:
                DEL_5();
                break;
            case 6:
                DEL_6();
                break;
            case 7:
                DEL_7();
                break;
            case 8:
                DEL_8();
                break;
            case 9:
                DEL_9();
                break;
        }
    }
}
