namespace LittleJacobMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using Interface;
using Saving;

internal class DeliveryMain : Script
{
    private struct HeadingPoint
    {
        public float X;
        public float Y;
        public float Z;
        public float Heading;
        public int Hash;
    }


    private enum PedTypes
    {
        Balla,
        Salva,
        Groove,
        Mex,
        Arm,
        Chi,
        Cartel,
        Biker,
        Billy,
        Other
    }
    public static bool Active { get; private set; }
    public static event EventHandler? OnDeliveryCompleted;
    private readonly RelationshipGroup _neutral;
    private readonly RelationshipGroup _hateCops;
    private readonly List<int> _peds = new();
    private readonly List<int> _vehicles = new();
    private int _lastSpawn;
    private int _deathToll;
    private readonly int _hate;
    private readonly int _cop;
    private Blip? _routeBlip;
    private Ped? _buyer;
    private Vector3 _destination;
    private Vehicle? _car;
    private int _objective;
    private int _blipSt;
    private Prop? _bag;
    private bool _bagTaken;
    private bool _pigFlag;
    private bool _betrayed;
    private int _lockedAt;
    private bool _carRecovered;
    private bool _fighting;
    private bool _highSpeed;
    private bool _startChase;
    private int _travelTime;
    private int _intensity;
    private bool _badDeal;
    private bool _rainyDay;
    private int _travelStartTime = 1;
    private float _health;
    private int _baseReward;
    private uint _pedModel;
    private PedTypes _chaserType;
    private uint _vehicleModel;
    private readonly Vector3 _dropPoint = new(6.828116f, -1405.562f, 28.26828f);

    private readonly Dictionary<int, Vector3> _badgeSlots = new()
    {
        {0, new Vector3(0.91f, 0.97f, 0)},
        {1, new Vector3(0.91f, 0.918f, 0)}
    };

    private readonly Dictionary<int, Vector3> _contentSlots = new()
    {
        {0, new Vector3(0.75f, 0.952f, 0)},
        {1, new Vector3(0.75f, 0.90f, 0)}
    };

    private readonly Dictionary<int, Vector3> _titleSlots = new()
    {
        {0, new Vector3(0.855f, 0.96f, 0)},
        {1, new Vector3(0.855f, 0.91f, 0)}
    };

    public DeliveryMain()
    {
        _neutral = World.AddRelationshipGroup("DRUG_DELIVERY_NEUTRAL_REL");
        _hateCops = World.AddRelationshipGroup("DRUG_DELIVERY_HATE_COPS_REL");
        _hate = Game.GenerateHash("HATES_PLAYER");
        _cop = Game.GenerateHash("COP");
        var player = Game.GenerateHash("PLAYER");
        _neutral.SetRelationshipBetweenGroups(player, Relationship.Neutral, true);
        _neutral.SetRelationshipBetweenGroups(_cop, Relationship.Neutral, true);
        _hateCops.SetRelationshipBetweenGroups(player, Relationship.Like, true);
        _hateCops.SetRelationshipBetweenGroups(_cop, Relationship.Hate, true);
        CallMenu.DeliverySelected += Start;
        Aborted += DeliveryMain_Aborted;
    }

    private static void LoadSprites()
    {
        Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, "timerbars", false);

        while (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, "timerbars"))
            Wait(1);
    }

    private static void UnloadSprites()
    {
        Function.Call(Hash.SET_STREAMED_TEXTURE_DICT_AS_NO_LONGER_NEEDED, "timerbars");
    }

    private static void DrawSprite(float x, float y, bool red)
    {
        if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, "timerbars"))
            LoadSprites();

        Function.Call(Hash.DRAW_SPRITE, "timerbars", "all_black_bg", x, y, 0.15f, 0.045f, 0, 0, 0, 0, 200);

        if (red)
            Function.Call(Hash.DRAW_SPRITE, "timerbars", "all_red_bg", x, y, 0.15f, 0.045f, 0, 165, 15, 1, 255);
    }

    private static void DrawSpriteText(float x, float y, float scale, string text, bool right)
    {
        Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
        Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);

        if (right)
        {
            Function.Call(Hash.SET_TEXT_WRAP, 0.6f, 0.975f);
            Function.Call(Hash.SET_TEXT_JUSTIFICATION, 2);
        }

        Function.Call(Hash.SET_TEXT_SCALE, 1.0f, scale);
        Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, x, y);
    }

    private void DrawBadge(string title, string content, bool red, int slot)
    {
        var bPos = _badgeSlots[slot];
        var cPos = _contentSlots[slot];
        var tPos = _titleSlots[slot];
        DrawSprite(bPos.X, bPos.Y, red);
        DrawSpriteText(cPos.X, cPos.Y, 0.42f, content, true);
        DrawSpriteText(tPos.X, tPos.Y, 0.295f, title, false);
    }

    private void DeliveryMain_Aborted(object sender, EventArgs e)
    {
        if (!Active) return;
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(5));
        ToggleMusicInterrup(false);
        UnloadSprites();
        Clean();
    }

    private static void RequestModel(int model)
    {
        Function.Call(Hash.REQUEST_MODEL, model);

        while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, model))
            Wait(1);
    }

    private static void RequestModel(uint model)
    {
        Function.Call(Hash.REQUEST_MODEL, model);

        while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, model))
            Wait(1);
    }

    private static string GetEvent(int param)
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
                return "VWC_MUSIC_STOP";
            case 5:
                return "VWC_FAIL";
            default:
                return "";
        }
    }

    private void Start(object o, EventArgs e)
    {
        var doc = XElement.Load($"{BaseDirectory}\\LittleJacobMod\\Missions\\drug_delivery.xml");
        var cars = doc.Element("CarLocations")?.Descendants("Location");
        var xElements = cars?.ToList();

        if (xElements == null) return;

        var size = xElements.Count();
        var points = new List<HeadingPoint>();
        var ran = new Random();

        for (var i = 0; i < size; i++)
        {
            var point = new HeadingPoint();
            var el = xElements.ElementAt(i);
            point.X = (float) el.Element("X");
            point.Y = (float) el.Element("Y");
            point.Z = (float) el.Element("Z");
            point.Heading = (float) el.Element("Heading");

            if (Game.Player.Character.IsInRange(new Vector3(point.X, point.Y, point.Z), 70))
                continue;

            points.Add(point);
        }

        var carPoint = points.ElementAt(ran.Next(0, points.Count));
        points.Clear();
        var buyers = doc.Elements("BuyerLocations").Descendants("Location");
        var enumerable = buyers.ToList();
        size = enumerable.Count();
        var validBuyers = new List<HeadingPoint>();
        var markers = new List<Vector3>();
        var carVector = new Vector3(carPoint.X, carPoint.Y, carPoint.Z);

        for (var i = 0; i < size; i++)
        {
            var el = enumerable.ElementAt(i);
            var buyer = el.Element("Buyer");
            var marker = el.Element("Marker");
            var markerPoint = new Vector3
            {
                X = (float) marker?.Element("X"),
                Y = (float) marker?.Element("Y"),
                Z = (float) marker?.Element("Z")
            };
            var dist = World.CalculateTravelDistance(carVector, markerPoint);

            if (dist < 1500)
                continue;

            var buyerPoint = new HeadingPoint
            {
                X = (float) buyer?.Element("X"),
                Y = (float) buyer?.Element("Y"),
                Z = (float) buyer?.Element("Z"),
                Heading = (float) buyer?.Element("Heading"),
                Hash = (int) buyer?.Element("Hash")
            };
            validBuyers.Add(buyerPoint);
            markers.Add(markerPoint);
        }

        if (validBuyers.Count == 0)
        {
            Notification.Show(NotificationIcon.Default, "Little Jacob", "Delivery",
                "Sorry man. I don't have any deliveries to make.");
        }
        else
        {
            var index = ran.Next(0, validBuyers.Count);
            var selectedBuyer = validBuyers.ElementAt(index);
            RequestModel(selectedBuyer.Hash);
            _buyer = Function.Call<Ped>(Hash.CREATE_PED, 0, selectedBuyer.Hash, selectedBuyer.X, selectedBuyer.Y,
                selectedBuyer.Z, selectedBuyer.Heading, false, false);
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, selectedBuyer.Hash);
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, _buyer.Handle, true);
            _buyer.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
            _buyer.RelationshipGroup = _neutral;
            _buyer.Weapons.Give(WeaponHash.Pistol, 100, false, true);
            var bagLocation = _buyer.Position.Around(0.4f);
            RequestModel(3898412430);
            _bag = Function.Call<Prop>(Hash.CREATE_OBJECT, 3898412430, bagLocation.X, bagLocation.Y,
                bagLocation.Z, false, false, false);
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, 3898412430);
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, _bag.Handle, true);
            Function.Call(Hash.PLACE_OBJECT_ON_GROUND_PROPERLY, _bag.Handle);
            _destination = markers.ElementAt(index);
            uint vHash;
            if (ran.Next(0, 101) <= DeliverySaving.HighSpeedChance)
            {
                vHash = (uint) VehicleHash.Buffalo4;
                _highSpeed = true;
                _baseReward = 65000;
            }
            else
            {
                vHash = (uint) VehicleHash.Tornado3;
                _highSpeed = false;
                _baseReward = 30000;
            }
            RequestModel(vHash);
            _car = Function.Call<Vehicle>(Hash.CREATE_VEHICLE, vHash, carPoint.X, carPoint.Y,
                carPoint.Z, carPoint.Heading, false, false);
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, vHash);
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

    private void SetModels(bool belongs)
    {
        var buyerModel = Function.Call<uint>(Hash.GET_ENTITY_MODEL, _buyer!.Handle);
        _chaserType = ThrowInPool(buyerModel);

        if (!belongs)
            _chaserType = GetEnemy(_chaserType);
        
        switch (_chaserType)
        {
            case PedTypes.Arm:
                _pedModel = (uint) PedHash.ArmGoon01GMM;
                _vehicleModel = (uint) VehicleHash.Schafter2;
                break;
            case PedTypes.Balla:
                _pedModel = (uint) PedHash.BallaOrig01GMY;
                _vehicleModel = (uint) VehicleHash.SabreGT2;
                break;
            case PedTypes.Biker:
                _pedModel = (uint) PedHash.Lost01GMY;
                _vehicleModel = (uint) VehicleHash.Daemon2;
                break;
            case PedTypes.Cartel:
                _pedModel = (uint) PedHash.PoloGoon01GMY;
                _vehicleModel = (uint) VehicleHash.Baller6;
                break;
            case PedTypes.Chi:
                _pedModel = (uint) PedHash.ChiGoon02GMM;
                _vehicleModel = (uint) VehicleHash.Cavalcade2;
                break;
            case PedTypes.Groove:
                _pedModel = (uint) PedHash.Families01GFY;
                _vehicleModel = (uint) VehicleHash.Baller3;
                break;
            case PedTypes.Mex:
                _pedModel = (uint) PedHash.MexGoon01GMY;
                _vehicleModel = (uint) VehicleHash.Phoenix;
                break;
            case PedTypes.Salva:
                _pedModel = (uint) PedHash.SalvaGoon01GMY;
                _vehicleModel = (uint) VehicleHash.Faction3;
                break;
            case PedTypes.Billy:
                _pedModel = (uint) PedHash.Hillbilly01AMM;
                _vehicleModel = (uint) VehicleHash.BfInjection;
                break;
            case PedTypes.Other:
                _pedModel = (uint) PedHash.Blackops01SMY;
                _vehicleModel = (uint) VehicleHash.Baller6;
                break;
        }
    }

    private static PedTypes GetEnemy(PedTypes type)
    {
        var intType = (int) type;
        var num = ExclusiveRanNum(intType, 0, 10);
        return (PedTypes) num;
    }

    private static int ExclusiveRanNum(int num, int min, int max)
    {
        var ran = new Random();
        var it = 0;
        while (true)
        {
            var val = ran.Next(min, max);
            if (val == num)
            {
                it++;
                if (it > 1000)
                    return 9;
                continue;
            }
            
            return val;
        }
    }

    private static PedTypes ThrowInPool(uint model)
    {
        switch (model)
        {
            case (uint)PedHash.MexGoon01GMY:
            case (uint)PedHash.MexGoon02GMY:
            case (uint)PedHash.MexGoon03GMY:
            case (uint)PedHash.MexBoss01GMM:
            case (uint)PedHash.MexBoss02GMM:
            case (uint)PedHash.Vagos01GFY:
                return PedTypes.Mex;
            
            case (uint)PedHash.Ballasog:
            case (uint)PedHash.BallasLeader:
            case (uint)PedHash.BallasogCutscene:
            case (uint)PedHash.Ballas01GFY:
            case (uint)PedHash.BallaEast01GMY:
            case (uint)PedHash.BallaOrig01GMY:
            case (uint)PedHash.BallaSout01GMY:
                return PedTypes.Balla;
            
            case (uint)PedHash.ArmGoon01GMM:
            case (uint)PedHash.ArmGoon02GMY:
            case (uint)PedHash.ArmBoss01GMM:
            case (uint)PedHash.ArmLieut01GMM:
                return PedTypes.Arm;
            
            case (uint)PedHash.Families01GFY:
            case (uint)PedHash.Famca01GMY:
            case (uint)PedHash.Famdnf01GMY:
            case (uint)PedHash.Famfor01GMY:
                return PedTypes.Groove;
            
            case (uint)PedHash.SalvaBoss01GMY:
            case (uint)PedHash.SalvaGoon01GMY:
            case (uint)PedHash.SalvaGoon02GMY:
            case (uint)PedHash.SalvaGoon03GMY:
                return PedTypes.Salva;
            
            case (uint)PedHash.ChiGoon01GMM:
            case (uint)PedHash.ChiGoon02GMM:
            case (uint)PedHash.ChinGoonCutscene:
            case (uint)PedHash.ChiBoss01GMM:
                return PedTypes.Chi;
            
            case (uint)PedHash.PoloGoon01GMY:
            case (uint)PedHash.PoloGoon02GMY:
            case (uint)PedHash.CartelGuards01GMM:
            case (uint)PedHash.CartelGuards02GMM:
                return PedTypes.Cartel;
            
            case (uint)PedHash.Lost01GFY:
            case (uint)PedHash.Lost01GMY:
            case (uint)PedHash.Lost02GMY:
            case (uint)PedHash.Lost03GMY:
            case (uint)PedHash.BikerChic:
                return PedTypes.Biker;
            
            case (uint) PedHash.Hillbilly01AMM:
            case (uint) PedHash.Hillbilly02AMM:
            case (uint) PedHash.Taphillbilly:
                return PedTypes.Billy;

            default:
                return PedTypes.Other;
        }
    }
    
    private void CreateDestinationBlip(int type)
    {
        _routeBlip = World.CreateBlip(_destination);
        _routeBlip.Scale = 0.8f;
        _routeBlip.Color = BlipColor.Yellow;
        _routeBlip.Name = type == 0 ? "Buyer" : "Drop point";
        _routeBlip.ShowRoute = true;
        _blipSt = Game.GameTime;
    }

    private void CreateCarBlip()
    {
        if (_car == null) return;
        _car.AddBlip();
        _car.AttachedBlip.Scale = 0.8f;
        _car.AttachedBlip.Sprite = BlipSprite.Weed;
        _car.AttachedBlip.Color = BlipColor.Green;
        _car.AttachedBlip.Name = "Jacob's car";
    }

    private void LoadChaserAssets()
    {
        RequestModel(_vehicleModel);
        RequestModel(_pedModel);
    }

    private void FreeChaserAssets()
    {
        Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, _pedModel);
        Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, _vehicleModel);
    }

    private VehicleColor GetVehicleColor(PedTypes type)
    {
        return type switch
        {
            PedTypes.Balla => VehicleColor.MetallicPurple,
            PedTypes.Groove => VehicleColor.MetallicLime,
            PedTypes.Mex => VehicleColor.MetallicRaceYellow,
            _ => VehicleColor.Blue
        };
    }
    
    private void SpawnChaser(float range)
    {
        if (_deathToll >= 3 || _vehicles.Count >= 3)
            return;
        
        if (Game.GameTime - _lastSpawn < 30000)
            return;
        
        var outPos = new OutputArgument();
        var arPPos = Game.Player.Character.Position.Around(range);
        var result = Function.Call<bool>(Hash.GET_CLOSEST_VEHICLE_NODE, arPPos.X, arPPos.Y, arPPos.Z, outPos, 0, 3, 0);
        if (!result)
            return;
        var vCoords = outPos.GetResult<Vector3>();
        var v = Function.Call<int>(Hash.CREATE_VEHICLE, _vehicleModel, vCoords.X, vCoords.Y, vCoords.Z, 0, false, false);

        switch (_chaserType)
        {
            case PedTypes.Balla:
            case PedTypes.Groove:
            case PedTypes.Mex:
                Function.Call(Hash.SET_VEHICLE_MOD_KIT, v, 0);
                var color = GetVehicleColor(_chaserType);
                Function.Call(Hash.SET_VEHICLE_COLOURS, v, (int) color, (int) color);
                break;
        }
        
        Function.Call(Hash.SET_VEHICLE_ENGINE_ON, v, true, true, false);
        var blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, v);
        blip.Scale = 0.85f;
        blip.Sprite = BlipSprite.Enemy;
        blip.Color = BlipColor.Red;
        blip.Name = "Enemy Vehicle";
        var seats = Function.Call<int>(Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, v);

        for (var i = -1; i < seats; i++)
        {
            var ped = Function.Call<int>(Hash.CREATE_PED_INSIDE_VEHICLE, v, 0, _pedModel, i, false, false);
            Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, ped, _hate);
            Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, (uint)WeaponHash.MicroSMG, 2000, false, true);

            if (i == -1)
            {
                Function.Call(Hash.TASK_VEHICLE_CHASE, ped, Main.PPID);
                Function.Call(Hash.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG, ped, 1, true);
            }
            else
            {
                Function.Call(Hash.TASK_VEHICLE_SHOOT_AT_PED, ped, Main.PPID, 40f);
                Function.Call(Hash.SET_PED_ACCURACY, ped, 30);
            }

            _peds.Add(ped);
        }

        _vehicles.Add(v);
        Notification.Show($"PED {_pedModel} VEHICLE {_vehicleModel} 3");
        _lastSpawn = Game.GameTime;
    }
    
    private void RemoveChasers(float distance)
    {
        for (var i = _peds.Count - 1; i > -1; i--)
        {
            var ped = _peds[i];
            var coords = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, ped);

            if (Game.Player.Character.IsInRange(coords, distance) && !Function.Call<bool>(Hash.IS_ENTITY_DEAD, ped)) continue;
            unsafe
            {
                Function.Call(Hash.SET_PED_AS_NO_LONGER_NEEDED, &ped);
            }
            _peds.RemoveAt(i);
        }

        for (var i = _vehicles.Count - 1; i > -1; i--)
        {
            var v = _vehicles[i];
            var coords = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, v);

            if (Game.Player.Character.IsInRange(coords, distance) && !Function.Call<bool>(Hash.IS_ENTITY_DEAD, v)) continue;
            unsafe
            {
                Function.Call(Hash.SET_VEHICLE_AS_NO_LONGER_NEEDED, &v);
            }
            _vehicles.RemoveAt(i);
            _deathToll++;
        }
    }
    
    private void DEL_0()
    {
        if (_buyer == null || _car == null)
        {
            Main.ShowScaleform("~r~Delivery failed", "", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }
        
        if (_buyer.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Buyer is dead", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }

        if (_car.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Car Destroyed", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }

        switch (Game.Player.WantedLevel)
        {
            case > 0 when !_pigFlag:
                _pigFlag = true;
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
                _car.AttachedBlip.Delete();
                return;
            case > 0 when _pigFlag:
                Screen.ShowSubtitle("Lose the cops.");
                return;
            case 0 when _pigFlag:
                _pigFlag = false;
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(-1));
                CreateCarBlip();
                break;
        }

        Screen.ShowSubtitle("Get in the ~g~car.", 1000);

        if (!Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Main.PPID, _car.Handle, false))
            return;

        _car.AttachedBlip.Delete();
        var ran = new Random();

        if (ran.Next(1, 101) <= (_highSpeed ? DeliverySaving.PoliceChanceLow + 6 : DeliverySaving.PoliceChanceLow))
        {
            Screen.ShowHelpText("The police were watching this car. Lose them!", 8000);
            Game.Player.WantedLevel = 2;
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            var distance = World.CalculateTravelDistance(Game.Player.Character.Position, _destination);
            _travelTime = (int) Math.Ceiling(distance * 60);

            if (_travelTime > 330000)
                _travelTime = 330000;
            if (_highSpeed)
                _travelTime = (int)Math.Ceiling(_travelTime / 1.5f);
            _travelTime += 60000;
            _travelStartTime = Game.GameTime;
            _objective = 2;
        }
        else
        {
            if (ran.Next(0, 101) <= (_highSpeed ? DeliverySaving.StartChaseChance + 10 : DeliverySaving.StartChaseChance))
            {
                SetModels(false);
                LoadChaserAssets();
                _startChase = true;
                _lastSpawn = Game.GameTime;
                Notification.Show(
                    NotificationIcon.Default,
                    "Little Jacob",
                    "Delivery",
                    "You got the car already? Word about the deal spread, if you see any fools" +
                    " following around you know what to do"
                );
            }
            CreateDestinationBlip(0);
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(1));
            var distance = World.CalculateTravelDistance(Game.Player.Character.Position, _destination);
            _travelTime = (int) Math.Ceiling(distance * 60);
            if (_travelTime > 330000)
            {
                _travelTime = 330000;
                if (_highSpeed)
                    _travelTime = (int)Math.Ceiling(_travelTime / 1.5f);
            }
            else
            {
                if (_highSpeed)
                    _travelTime = (int)Math.Ceiling(_travelTime / 1.5f);
                _travelTime += 20000;
            }
            _travelStartTime = Game.GameTime;
            _objective = 1;
        }
    }

    private void DEL_1()
    {
        if (_buyer == null || _car == null)
        {
            Main.ShowScaleform("~r~Delivery failed", "", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }
        
        if (_buyer.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Buyer is dead", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }

        if (_car.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Car Destroyed", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }

        if (Game.Player.WantedLevel > 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            _routeBlip?.Delete();
            _objective = 2;
            return;
        }

        if (_startChase)
        {
            SpawnChaser(120);
            RemoveChasers(5000);
        }
        
        Screen.ShowSubtitle("Go to the ~y~buyer.", 1000);
        Function.Call(Hash.DRAW_MARKER, 1, _destination.X, _destination.Y, _destination.Z, 0, 0, 0, 0, 0,
            0, 5f, 5f, 2f, 255, 255, 0, 100, false, false, 2, false, false, false);

        if (_car.IsInRange(_destination, 5))
        {
            _routeBlip?.Delete();
            var ran = new Random();

            if (ran.Next(0, 101) <= DeliverySaving.PoliceChanceLow)
            {
                if (ran.Next(0, 101) <= DeliverySaving.PoliceChanceHigh)
                {
                    Screen.ShowHelpText("The buyer is a cop. Get out of there!", 16000);
                    Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, _buyer.Handle, _cop);
                    Function.Call(Hash.SET_PED_AS_COP, _buyer.Handle, true);
                    _buyer.Task.ClearAllImmediately();
                    _buyer.Task.FightAgainst(Game.Player.Character);
                    _rainyDay = true;
                    Game.Player.WantedLevel = 4;
                }
                else
                {
                    Screen.ShowHelpText("The cops were alerted of the deal. Lose them!", 16000);
                    _buyer.RelationshipGroup = _hateCops;
                    _buyer.Task.ClearAllImmediately();
                    _buyer.Task.FleeFrom(_destination);
                    Game.Player.WantedLevel = 3;
                }

                _rainyDay = true;
                _objective = 5;
                _destination = _dropPoint;
            }
            else
            {
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(-1));
                Game.Player.Character.Task.LeaveVehicle();
                _car.Velocity = Vector3.Zero;
                _car.LockStatus = VehicleLockStatus.PlayerCannotEnter;
                if (_bag != null)
                {
                    _bag.AddBlip();
                    _bag.AttachedBlip.Scale = 0.75f;
                    _bag.AttachedBlip.Color = BlipColor.Green;
                    _bag.AttachedBlip.Name = "Money";
                }
                Wait(1000);
                var sequence = new TaskSequence();
                sequence.AddTask.ClearAllImmediately();
                sequence.AddTask.EnterVehicle(_car, VehicleSeat.Driver);
                sequence.AddTask.CruiseWithVehicle(_car, 50, DrivingStyle.Rushed);
                _buyer.Task.PerformSequence(sequence);
                _objective = 4;
            }

            return;
        }
        
        if (!Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Main.PPID, _car.Handle, false))
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(-1));
            _routeBlip?.Delete();
            CreateCarBlip();
            _objective = 3;
            return;
        }
        
        if (!_car.IsInRange(_destination, 300)) return;
        RemoveChasers(0.1f);
    }

    private void DEL_2()
    {
        if (_buyer == null || _car == null)
        {
            Main.ShowScaleform("~r~Delivery failed", "", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }
        
        var farAway = !Game.Player.Character.IsInRange(_car.Position, 100);

        if (farAway)
            _car.IsConsideredDestroyed = true;

        if (_buyer.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Buyer is dead", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }

        if (_car.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", farAway ? "Car abandoned" : "Car Destroyed", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }

        if (_startChase)
        {
            RemoveChasers(5000);
        }
        
        Screen.ShowSubtitle("Lose the cops.", 1000);

        if (Game.Player.WantedLevel == 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(_intensity));
            CreateCarBlip();
            _objective = 3;
            return;
        }

        if (_car.IsInRange(_destination, 20))
        {
            var ran = new Random();

            if (ran.Next(0, 101) <= DeliverySaving.PoliceChanceHigh)
            {
                Screen.ShowHelpText("The buyer is a cop. Get out of there!", 16000);
                Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, _buyer.Handle, _cop);
                Function.Call(Hash.SET_PED_AS_COP, _buyer.Handle, true);
                _buyer.Task.FightAgainst(Game.Player.Character);
                Game.Player.WantedLevel = 4;
                _rainyDay = true;
                _objective = 5;
                _destination = _dropPoint;
            }
            else
            {
                if (!Function.Call<bool>(Hash.ARE_PLAYER_STARS_GREYED_OUT, Game.Player.Handle))
                {
                    _buyer.Task.FleeFrom(_destination);
                    _destination = _dropPoint;
                    Screen.ShowHelpText("Deal canceled. Buyer was scared.", 16000);
                    _objective = 5;
                }
                else
                {
                    Game.Player.Character.Task.LeaveVehicle();
                    _car.Velocity = Vector3.Zero;
                    _car.LockStatus = VehicleLockStatus.PlayerCannotEnter;
                    if (_bag != null)
                    {
                        _bag.AddBlip();
                        _bag.AttachedBlip.Scale = 0.75f;
                        _bag.AttachedBlip.Color = BlipColor.Green;
                        _bag.AttachedBlip.Name = "Money";
                    }
                    Wait(1000);
                    var sequence = new TaskSequence();
                    sequence.AddTask.ClearAllImmediately();
                    sequence.AddTask.CruiseWithVehicle(_car, 50, DrivingStyle.Rushed);
                    _buyer.Task.PerformSequence(sequence);
                    _objective = 4;
                }
            }
            return;
        }
        
        if (!_car.IsInRange(_destination, 300)) return;
        RemoveChasers(0.1f);
    }

    private void DEL_3()
    {
        if (_buyer == null || _car == null)
        {
            Main.ShowScaleform("~r~Delivery failed", "", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }
        
        if (_buyer.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Buyer is dead", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }

        if (_car.IsDead)
        {
            Main.ShowScaleform("~r~Delivery failed", "Car Destroyed", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }

        if (Game.Player.WantedLevel > 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            _car.AttachedBlip.Delete();
            _objective = 2;
            return;
        }

        if (_startChase)
        {
            SpawnChaser(120);
            RemoveChasers(5000);
        }
        Screen.ShowSubtitle("Go back to the ~g~car.", 1000);

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

    private void DEL_4()
    {
        if (_buyer == null || _car == null)
        {
            Main.ShowScaleform("~r~Delivery failed", "", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }
        
        Screen.ShowSubtitle("Take the ~g~money~w~ and leave the area.", 1000);
        var buyerLeft = !_buyer.IsInRange(_destination, 50) || !_buyer.IsInRange(Game.Player.Character.Position, 50);

        switch (_bagTaken)
        {
            case true when buyerLeft:
                Clean();
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(-1));
                _destination = _dropPoint;
                CreateDestinationBlip(1);
                _objective = 8;
                break;
            case true when _buyer.IsDead:
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
                _car.LockStatus = VehicleLockStatus.None;
                _destination = _dropPoint;
                _betrayed = true;
                _carRecovered = true;
                _deathToll = 0;
                FreeChaserAssets();
                SetModels(true);
                LoadChaserAssets();
                _objective = 6;
                break;
        }

        if (_fighting) return;
        if (!Game.Player.Character.IsTryingToEnterALockedVehicle ||
            Game.Player.Character.VehicleTryingToEnter != _car) return;
        _fighting = true;
        _buyer.Task.ClearAll();
        _buyer.Task.FightAgainst(Game.Player.Character);
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(2));
    }

    private void DEL_5()
    {
        if (_car == null)
        {
            Main.ShowScaleform("~r~Delivery failed", "", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }
        
        var farAway = !Game.Player.Character.IsInRange(_car.Position, 100);

        if (farAway)
            _car.IsConsideredDestroyed = true;

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
                Main.ShowScaleform("~r~Mission failed", farAway ? "Car abandoned" : "Car Destroyed", 0);
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
                Quit();
            }

            return;
        }

        Screen.ShowSubtitle("Lose the cops.", 1000);

        if (Game.Player.WantedLevel != 0) return;
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(1));
        CreateCarBlip();
        _objective = 6;
    }

    private void DEL_6()
    {
        if (_car == null)
        {
            Main.ShowScaleform("~r~Delivery failed", "", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }
        
        var farAway = !Game.Player.Character.IsInRange(_car.Position, 100);

        if (farAway)
            _car.IsConsideredDestroyed = true;

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
                Main.ShowScaleform("~r~Mission failed", farAway ? "Car abandoned" : "Car Destroyed", 0);
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
                Quit();
            }

            return;
        }

        if (Game.Player.WantedLevel > 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            _objective = 5;
            return;
        }

        if (_betrayed)
        {
            RemoveChasers(5000);
        }
        
        Screen.ShowSubtitle("Go back to the ~g~car.", 1000);

        if (!Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Main.PPID, _car.Handle, false))
            return;

        if (_car.AttachedBlip != null)
        {
            _car.AttachedBlip.Delete();
        }

        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(_bagTaken ? 2 : 1));
        CreateDestinationBlip(1);
        _lastSpawn = Game.GameTime;
        _objective = 7;
    }

    private void DEL_7()
    {
        if (_car == null)
        {
            Main.ShowScaleform("~r~Delivery failed", "", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
            Quit();
            return;
        }
        
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
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
                Quit();
            }

            return;
        }

        if (Game.Player.WantedLevel > 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
            _routeBlip?.Delete();
            _objective = 5;
            return;
        }
        
        if (_betrayed)
        {
            SpawnChaser(120);
            RemoveChasers(5000);
        }

        Screen.ShowSubtitle("Take the car to the ~y~drop point.", 1000);
        Function.Call(Hash.DRAW_MARKER, 1, _destination.X, _destination.Y, _destination.Z, 0, 0, 0, 0, 0,
            0, 5f, 5f, 2f, 255, 255, 0, 100, false, false, 2, false, false, false);

        if (_car.IsInRange(_destination, 5))
        {
            _car.Velocity = Vector3.Zero;
            Game.Player.Character.Task.LeaveVehicle();
            _car.LockStatus = VehicleLockStatus.PlayerCannotEnter;
            Complete();
        }
        else if (!Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Main.PPID, _car.Handle, false))
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(-1));
            _routeBlip?.Delete();
            CreateCarBlip();
            _objective = 6;
        }
        
        if (!_car.IsInRange(_destination, 250)) return;
        RemoveChasers(0.1f);
    }

    private void DEL_8()
    {
        if (Game.Player.WantedLevel > 0)
        {
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));

            if (_routeBlip != null && _routeBlip.Handle != 0)
                _routeBlip.Delete();

            _objective = 9;
            return;
        }

        if (_betrayed)
        {
            SpawnChaser(120);
            RemoveChasers(5000);
        }
        
        Screen.ShowSubtitle("Deliver the money to Jacob.");
        Function.Call(Hash.DRAW_MARKER, 1, _destination.X, _destination.Y, _destination.Z, 0, 0, 0, 0, 0,
            0, 5f, 5f, 2f, 255, 255, 0, 100, false, false, 2, false, false, false);

        if (Game.Player.Character.IsInRange(_destination, 5) && !Game.Player.Character.IsInVehicle())
            Complete();
        
        if (!Game.Player.Character.IsInRange(_destination, 250)) return;
        RemoveChasers(0.1f);
    }

    private void DEL_9()
    {
        if (_betrayed)
        {
            SpawnChaser(120);
            RemoveChasers(5000);
        }
        Screen.ShowSubtitle("Lose the cops.");
        if (Game.Player.WantedLevel != 0) return;
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(-1));
        CreateDestinationBlip(1);
        _objective = 8;
    }

    private static void ToggleMusicInterrup(bool value)
    {
        Function.Call(Hash.SET_AUDIO_FLAG, "DisableFlightMusic", value);
        Function.Call(Hash.SET_AUDIO_FLAG, "WantedMusicDisabled", value);
    }

    private void Complete()
    {
        OnDeliveryCompleted?.Invoke(this, EventArgs.Empty);

        if (_bagTaken && _carRecovered)
        {
            var multiplier = _health / 10 * 0.01f;
            var carBonus = (int) (_baseReward * multiplier);
            Game.Player.Money += _lockedAt + carBonus;

            switch (_rainyDay)
            {
                case true when !_badDeal:
                    DeliverySaving.GoodDeal();
                    break;
                case false when _badDeal:
                    DeliverySaving.GoodDeal();
                    break;
                case false when !_badDeal:
                    DeliverySaving.BadDeal();
                    break;
            }
        }
        else
        {
            Game.Player.Money += _lockedAt;
            
            if (!_betrayed)
                DeliverySaving.GoodDeal();
            else
                DeliverySaving.BadDeal();
        }
        DeliverySaving.DealCount++;
        if (DeliverySaving.DealCount == 3)
        {
            DeliverySaving.DealCount = 0;
            if (DeliverySaving.BaseChaseChance < 15)
            {
                DeliverySaving.BaseChaseChance++;   
            }

            if (DeliverySaving.BaseHighSpeedChance < 30)
            {
                DeliverySaving.BaseHighSpeedChance++;
            }
        }
        DeliverySaving.Save();
        Main.ShowScaleform("~g~Delivery Completed", "", 0);
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(4));
        Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", true);
        ToggleMusicInterrup(false);
        Clean();
        UnloadSprites();
        Active = false;
        Tick -= DeliveryMain_Tick;
    }

    private void Quit()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(5));
        ToggleMusicInterrup(false);
        Clean();
        UnloadSprites();
        Active = false;
        Tick -= DeliveryMain_Tick;
    }

    private void Clean()
    {
        _rainyDay = false;
        _badDeal = false;
        _betrayed = false;
        _deathToll = 0;
        _startChase = false;
        if (_car != null && _car.Handle != 0)
            _car.MarkAsNoLongerNeeded();

        if (_buyer != null && _buyer.Handle != 0)
            _buyer.MarkAsNoLongerNeeded();

        if (_routeBlip != null && _routeBlip.Handle != 0)
            _routeBlip.Delete();

        if (_bag != null && _bag.Handle != 0)
            _bag.Delete();
        for (var i = _peds.Count - 1; i > -1; i--)
        {
            var ped = _peds[i];
            unsafe
            {
                Function.Call(Hash.SET_PED_AS_NO_LONGER_NEEDED, &ped);
            }
            _peds.RemoveAt(i);
        }
        for (var i = _vehicles.Count - 1; i > -1; i--)
        {
            var v = _vehicles[i];
            unsafe
            {
                Function.Call(Hash.SET_VEHICLE_AS_NO_LONGER_NEEDED, &v);
            }
            _vehicles.RemoveAt(i);
        }
        FreeChaserAssets();
    }

    private static string ColorModifier(int value, int baseVal)
    {
        if (value >= baseVal / 3 * 2)
            return "~g~";
        return value >= baseVal / 3 ? "~y~" : "~r~";
    }

    private void HideHUD()
    {
        Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 6);
        Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 7);
        Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 8);
        Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 9);
    }
    
    private void DeliveryMain_Tick(object sender, EventArgs e)
    {
        if (!Active)
            return;

        if (!Main.ScaleformActive && _objective == -1)
        {
            Notification.Show(NotificationIcon.Default, "Little Jacob", "Delivery",
                "Ok, rasta. Just get the ~g~car~w~ and deliver it to the ~y~buyer~w~. The ~y~buyer~w~ is waiting so you better hurry.");
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
            if (Game.GameTime - _blipSt >= 2000)
            {
                _routeBlip.ShowRoute = true;
                _blipSt = Game.GameTime;
            }
        }

        if ((_car?.EngineHealth ?? 0) < _health)
            _health = _car?.EngineHealth ?? 0;

        if (_objective > 0)
        {
            var mod = ColorModifier(_lockedAt, _carRecovered ? _baseReward * 2 : _baseReward);

            if (!_bagTaken)
            {
                if (_car == null || _bag == null)
                {
                    Main.ShowScaleform("~r~Delivery failed", "", 0);
                    Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
                    Quit();
                    return;
                }
                
                var multiplier = _health / 10 * 0.01f;
                _lockedAt = (int) (_baseReward * multiplier);

                if (Game.Player.Character.IsInRange(_bag.Position, 1.25f) && !Game.Player.Character.IsInVehicle())
                {
                    Screen.ShowHelpTextThisFrame("Press ~INPUT_CONTEXT~ to pick up the ~g~money.", false);

                    if (Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, 51))
                    {
                        Game.Player.Character.Task.PlayAnimation("pickup_object", "pickup_low");
                        Wait(1000);
                        Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Bus_Schedule_Pickup",
                            "DLC_PRISON_BREAK_HEIST_SOUNDS", false);
                        _bag.Delete();
                        _bagTaken = true;

                        if (_objective > 4 && !_carRecovered && !_car.IsDead)
                            _carRecovered = true;
                    }
                }

                DrawBadge($"{mod}REWARD", $"{mod}${_lockedAt.ToString()}", mod == "~r~", 0);
                HideHUD();
            }
            else
            {
                if (_carRecovered)
                {
                    var multiplier = _health / 10 * 0.01f;
                    var carBonus = (int) (_baseReward * multiplier);
                    mod = ColorModifier(_lockedAt + carBonus, 160000);
                    DrawBadge($"{mod}REWARD", $"{mod}${(_lockedAt + carBonus).ToString()}", mod == "~r~", 0);
                }
                else
                {
                    DrawBadge($"{mod}REWARD", $"{mod}${_lockedAt.ToString()}", mod == "~r~", 0);
                }
                HideHUD();
            }

            if (_objective < 4)
            {
                var remaining = _travelTime - (Game.GameTime - _travelStartTime);

                if (Game.GameTime - _travelStartTime >= _travelTime)
                {
                    Main.ShowScaleform("~r~Delivery Failed", "The buyer left.", 0);
                    Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
                    Quit();
                    return;
                }

                string colorModifier;
                var red = false;

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

                var title = $"{colorModifier}TIME LEFT";
                var content = $"{colorModifier}";
                var val = remaining / 1000 / 60;

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