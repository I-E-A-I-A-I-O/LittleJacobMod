using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using GTA;
using GTA.Native;
using GTA.Math;
using LittleJacobMod.Interface;
using LittleJacobMod.Utils;
using LittleJacobMod.Saving;

namespace LittleJacobMod
{
    class MissionMain : Script
    {
        enum Missions : uint
        {
            FM1 = 274994068,
            FM2 = 766266916,
            FM3 = 4044018910,
            MM1 = 1765589394,
            MM2 = 1526474001,
            MM3 = 1290406125
        }

        readonly List<WeaponHash> _weapons = new List<WeaponHash>()
        {
            WeaponHash.AssaultrifleMk2,
            WeaponHash.PumpShotgunMk2,
            WeaponHash.CombatMGMk2,
            WeaponHash.SMGMk2,
            WeaponHash.DoubleBarrelShotgun,
            WeaponHash.AssaultShotgun
        };

        struct Position
        {
            public Vector3 Location;
            public Vector3 Rotation;
        }

        struct ModelType
        {
            public uint Hash;
            public int type;
        }

        struct SpawnData
        {
            public List<Position> Positions;
            public List<ModelType> ModelTypes;
        }

        public static bool Active { get; private set; }
        public static event EventHandler OnMissionCompleted;
        List<int> _peds = new List<int>();
        List<int> _vehicles = new List<int>();
        List<int> _props = new List<int>();
        List<Blip> _blips = new List<Blip>();
        List<Vector3> _locations = new List<Vector3>();
        SpawnData _data;
        int _objective;
        bool _spawned;
        bool _copsAlerted;
        bool _increased;
        string _dir;
        Missions _mission;
        Controls CancelMissionKey;
        RelationshipGroup _dislike;
        RelationshipGroup _neutral;
        int _targetV;
        int _scaleform;
        bool _scaleFormActive;
        int _scaleFormStart;
        bool _chaserTS;
        int _chaserST;
        bool _scaleFormFading;
        bool _objtvCrtd;
        bool _routeTA;
        bool _TookDrugs;
        bool _blipOff;
        bool _fightFlag;
        bool _fail;
        int _routeST;
        uint _pedModel;
        Random _ran;

        public MissionMain()
        {
            var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
            _dir = $"{BaseDirectory}\\LittleJacobMod\\Missions";
            CancelMissionKey = settings.GetValue("Controls", "CancelMission", Controls.INPUT_SWITCH_VISOR);
            int playerRel = Game.GenerateHash("PLAYER");
            _dislike = World.AddRelationshipGroup("JACOB_MISSION_REL_DISLIKE");
            _neutral = World.AddRelationshipGroup("JACOB_MISSION_REL_NEUTRAL");
            _dislike.SetRelationshipBetweenGroups(playerRel, Relationship.Dislike, true);
            _neutral.SetRelationshipBetweenGroups(playerRel, Relationship.Neutral, true);
            _ran = new Random();
            Tick += OnTick;
            Aborted += MissionMain_Aborted;
            CallMenu.JobSelected += Start;
        }

        private void MissionMain_Aborted(object sender, EventArgs e)
        {
            if (Active)
                Quit();
        }

        void LoadFromFile(string name)
        {
            XElement missionData = XElement.Load($"{_dir}\\{name}.xml");
            _data = new SpawnData
            {
                ModelTypes = new List<ModelType>(),
                Positions = new List<Position>()
            };
            IEnumerable<XElement> elements = from item in missionData.Elements().Descendants("MapObject")
                          select item;
            IEnumerable<XElement> markers = from item in missionData.Elements().Descendants("Marker")
                                             select item;

            for (int i = 0; i < elements.Count(); i++)
            {
                XElement element = elements.ElementAt(i);
                Position pos = new Position();
                ModelType modelType = new ModelType();
                pos.Location.X = (float)element.Element("Position").Element("X");
                pos.Location.Y = (float)element.Element("Position").Element("Y");
                pos.Location.Z = (float)element.Element("Position").Element("Z");
                pos.Rotation.X = (float)element.Element("Rotation").Element("X");
                pos.Rotation.Y = (float)element.Element("Rotation").Element("Y");
                pos.Rotation.Z = (float)element.Element("Rotation").Element("Z");
                modelType.Hash = (uint)(int)element.Element("Hash");
                string type = (string)element.Element("Type");
                
                switch (type)
                {
                    case "Ped":
                        modelType.type = 0;
                        break;
                    case "Vehicle":
                        modelType.type = 1;
                        break;
                    case "Prop":
                        modelType.type = 2;
                        break;
                }

                _data.ModelTypes.Add(modelType);
                _data.Positions.Add(pos);
            }

            for (int i = 0; i < markers.Count(); i++)
            {
                XElement marker = markers.ElementAt(i);
                Vector3 pos = new Vector3
                {
                    X = (float)marker.Element("Position").Element("X"),
                    Y = (float)marker.Element("Position").Element("Y"),
                    Z = (float)marker.Element("Position").Element("Z")
                };
                _locations.Add(pos);
            }
        }

        void Start(object o, EventArgs e)
        {
            if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, PedHash.Franklin))
            {
                if (MissionSaving.FProgress < 4)
                {
                    _fail = false;
                    string misName = $"franklin_m_{MissionSaving.FProgress}";
                    _mission = (Missions)(uint)Game.GenerateHash(misName);
                    LoadFromFile(misName);

                    if (Game.Player.Character.IsInRange(_locations[0], 250))
                    {
                        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Jobs", "I have no jobs for you atm");
                        Clean();
                        return;
                    }

                    Active = true;
                    _objective = -1;
                    return;
                }
            } else if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, PedHash.Michael))
            {
                if (MissionSaving.MProgress < 4)
                {
                    _fail = false;
                    string misName = $"michael_m_{MissionSaving.MProgress}";
                    _mission = (Missions)(uint)Game.GenerateHash(misName);
                    LoadFromFile(misName);

                    if (Game.Player.Character.IsInRange(_locations[0], 250))
                    {
                        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Jobs", "I have no jobs for you atm");
                        Clean();
                        return;
                    }

                    if (_mission == Missions.MM2)
                        _neutral.SetRelationshipBetweenGroups(Game.GenerateHash("AMBIENT_GANG_LOST"), Relationship.Like, true);
                    else
                        _neutral.SetRelationshipBetweenGroups(Game.GenerateHash("AMBIENT_GANG_LOST"), Relationship.Neutral, true);

                    Active = true;
                    _objective = -1;
                    return;
                }
            } else if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, PedHash.Trevor))
            {
                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Jobs", "No.");
                return;
            }

            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Jobs", "I have no jobs for you atm");
        }

        void ResetFlags()
        {
            _spawned = false;
            _increased = false;
            _copsAlerted = false;
            _fightFlag = false;
            _objtvCrtd = false;
        }

        void SetScaleFormText(string title, string description)
        {
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, _scaleform, "SHOW_SHARD_CENTERED_TOP_MP_MESSAGE");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_TEXTURE_NAME_STRING, title);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_TEXTURE_NAME_STRING, description);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 5);
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
        }

        void FadeOutScaleform()
        {
            Function.Call(Hash.CALL_SCALEFORM_MOVIE_METHOD, _scaleform, "SHARD_ANIM_OUT");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 5);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 3000);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_BOOL, true);
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
        }

        int RequestScaleform()
        {
            _scaleform = Function.Call<int>(Hash.REQUEST_SCALEFORM_MOVIE, "mp_big_message_freemode");

            while (!Function.Call<bool>(Hash.HAS_SCALEFORM_MOVIE_LOADED, _scaleform))
                Wait(1);

            return _scaleform;
        }

        void FreeScaleform()
        {
            int s = _scaleform;
            unsafe
            {
                Function.Call(Hash.SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED, &s);
            }
        }

        void Complete()
        {
            MissionSaving.Save();
            OnMissionCompleted?.Invoke(this, EventArgs.Empty);
            Game.Player.Money += 80000;
            RequestScaleform();
            SetScaleFormText("~y~Mission passed", "");
            _scaleFormActive = true;
            _scaleFormStart = Game.GameTime;
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(4));
            ToggleMusicInterrup(false);
            Clean();
            ResetFlags();

            switch (_mission)
            {
                case Missions.FM1:
                    MissionSaving.FProgress += 1;
                    break;
                case Missions.FM2:
                    MissionSaving.FProgress += 1;
                    break;
                case Missions.FM3:
                    GTA.UI.Notification.Show("~g~Little Jacob mod~w~: 20% discount unlocked for Franklin");
                    MissionSaving.FProgress += 1;
                    break;
                case Missions.MM1:
                    MissionSaving.MProgress += 1;
                    break;
                case Missions.MM2:
                    MissionSaving.MProgress += 1;
                    break;
                case Missions.MM3:
                    GTA.UI.Notification.Show("~g~Little Jacob mod~w~: 20% discount unlocked for Michael");
                    MissionSaving.MProgress += 1;
                    break;
            }

            Active = false;
        }

        void Quit()
        {
            _fail = true;
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(5));
            ToggleMusicInterrup(false);
            Clean();
            ResetFlags();
            Active = false;
        }

        void Input()
        {

        }

        void Clean()
        {
            for (int i = 0; i < _peds.Count; i++)
            {
                int el = _peds[i];
                unsafe
                {
                    Function.Call(Hash.SET_PED_AS_NO_LONGER_NEEDED, &el);
                }
            }

            for (int i = 0; i < _vehicles.Count; i++)
            {
                int el = _vehicles[i];
                unsafe
                {
                    Function.Call(Hash.SET_VEHICLE_AS_NO_LONGER_NEEDED, &el);
                }
            }

            for (int i = 0; i < _props.Count; i++)
            {
                int el = _props[i];
                unsafe
                {
                    Function.Call(Hash.SET_OBJECT_AS_NO_LONGER_NEEDED, &el);
                }
            }

            for (int i = 0; i < _blips.Count; i++)
                _blips[i].Delete();

            _locations.Clear();
            _peds.Clear();
            _vehicles.Clear();
            _props.Clear();
            _blips.Clear();
            _data.ModelTypes.Clear();
            _data.Positions.Clear();

            if ((_mission == Missions.MM1 || _mission == Missions.MM2 || _mission == Missions.MM3) && _objective > 1)
            {
                FreeModel((uint)VehicleHash.Baller6);
                FreeModel(_pedModel);
            }

            if ((_mission == Missions.MM2 || _mission == Missions.MM3) && (_objective > 0 || _spawned))
            {
                int v = _targetV;

                unsafe
                {
                    Function.Call(Hash.SET_VEHICLE_AS_NO_LONGER_NEEDED, &v);
                }
            }
        }

        string MissionTitle()
        {
            switch(_mission)
            {
                case Missions.FM1:
                case Missions.FM2:
                case Missions.FM3:
                    return "~y~Assassination";
                case Missions.MM1:
                case Missions.MM2:
                case Missions.MM3:
                    return "~y~Repossession";
                default:
                    return "";
            }
        }

        string MissionSubtitle()
        {
            switch (_mission)
            {
                case Missions.FM1:
                    return "Vagos: Part 1";
                case Missions.FM2:
                    return "Vagos: Part 2";
                case Missions.FM3:
                    return "The Lost";
                case Missions.MM1:
                    return "Drugs";
                case Missions.MM2:
                    return "Sanctus";
                case Missions.MM3:
                    return "Phantom";
                default:
                    return "";
            }
        }

        void LoadModel(uint model)
        {
            Function.Call(Hash.REQUEST_MODEL, model);

            while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, model))
                Wait(1);
        }

        void FreeModel(uint model)
        {
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, model);
        }

        string GetEvent(int param)
        {
            switch(param)
            {
                case 0:
                    return "FIXER_SUSPENSE_START";
                case 1:
                    return "FIXER_MED_INTENSITY";
                case 2:
                    return "FIXER_GUNFIGHT";
                case 3:
                    return "FIXER_VEHICLE_ACTION";
                case 4:
                    return "MP_DM_COUNTDOWN_KILL";
                case 5:
                    return "FIXER_FAIL";
                default:
                    return "";
            }
        }

        void ToggleMusicInterrup(bool value)
        {
            Function.Call(Hash.SET_AUDIO_FLAG, "DisableFlightMusic", value);
            Function.Call(Hash.SET_AUDIO_FLAG, "WantedMusicDisabled", value);
        }

        void EnemyBlip(int ped)
        {
            Blip blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, ped);
            blip.Scale = 0.7f;
            blip.Sprite = BlipSprite.Enemy;
            blip.Color = BlipColor.Red;
            blip.Name = "Enemy";
        }

        bool AnyFightingPlayer()
        {
            for (int i = 0; i < _peds.Count; i++)
                if (Function.Call<bool>(Hash.IS_PED_IN_COMBAT, _peds[i], Main.PPID))
                    return true;

            return false;
        }

        void RandomWeapon(int ped)
        {
            int val = _ran.Next(0, _weapons.Count);
            Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, _weapons[val], 2000, false, true);
        }

        void RandomScenario(int ped)
        {
            int val = _ran.Next(0, 3);
            string scenario;

            switch (val)
            {
                case 0:
                    scenario = "WORLD_HUMAN_STAND_IMPATIENT";
                    break;
                case 1:
                    scenario = "WORLD_HUMAN_SMOKING";
                    break;
                default:
                    scenario = "WORLD_HUMAN_STAND_MOBILE";
                    break;
            }

            Function.Call(Hash.TASK_START_SCENARIO_IN_PLACE, ped, scenario, 0, true);
        }

        void RemoveFarEnts(float distance)
        {
            for (int i = _peds.Count - 1; i > -1; i--)
            {
                int ped = _peds[i];
                Vector3 coords = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, ped);

                if (!Game.Player.Character.IsInRange(coords, distance))
                {
                    unsafe
                    {
                        Function.Call(Hash.SET_PED_AS_NO_LONGER_NEEDED, &ped);
                    }
                    _peds.RemoveAt(i);
                }
            }

            for (int i = _vehicles.Count - 1; i > -1; i--)
            {
                int v = _vehicles[i];
                Vector3 coords = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, v);

                if (!Game.Player.Character.IsInRange(coords, distance))
                {
                    unsafe
                    {
                        Function.Call(Hash.SET_VEHICLE_AS_NO_LONGER_NEEDED, &v);
                    }
                    _vehicles.RemoveAt(i);
                }
            }
        }

        bool IsInRange(Vector3 a, Vector3 b, float range)
        {
            Vector3 nv3 = new Vector3();
            nv3.X = a.X - b.X;
            nv3.Y = a.Y - b.Y;
            nv3.Z = a.Z - b.Z;
            return (nv3.X * nv3.X) + (nv3.Y * nv3.Y) + (nv3.Z * nv3.Z) < range * range;
        }

        void RemoveDeadEnts()
        {
            for (int i = _peds.Count - 1; i > -1; i--)
            {
                int ped = _peds[i];

                if (Function.Call<bool>(Hash.IS_PED_DEAD_OR_DYING, ped, 1))
                {
                    unsafe
                    {
                        Function.Call(Hash.SET_PED_AS_NO_LONGER_NEEDED, &ped);
                    }
                    _peds.RemoveAt(i);
                }
            }

            for (int i = _vehicles.Count - 1; i > -1; i--)
            {
                int v = _vehicles[i];

                if (Function.Call<bool>(Hash.IS_ENTITY_DEAD, v))
                {
                    unsafe
                    {
                        Function.Call(Hash.SET_VEHICLE_AS_NO_LONGER_NEEDED, &v);
                    }
                    _vehicles.RemoveAt(i);
                }
            }
        }

        void ASS_MSG(int code)
        {
            string msg;

            switch(code)
            {
                case 0:
                    msg = "Go to the ~y~gas station~w~ and send the ~r~Vagos~w~ to mexican heaven";
                    break;
                case 1:
                    msg = "A group of ~r~Vagos~w~ is getting ready to make a move on me. Get em first.";
                    break;
                case 2:
                    msg = "These ~r~biker~w~ boys have been messing with my bussiness. Get rid of them.";
                    break;
                case 3:
                    msg = "Nice one my breden. Respect.";
                    break;
                default:
                    msg = "";
                    break;
            }

            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Job", msg);
        }

        void REP_MSG(int code)
        {
            string msg;

            switch (code)
            {
                case 0:
                    msg = "A few ~r~Ballas~w~ are moving some ~g~product~w~, bring it to me.";
                    break;
                case 1:
                    msg = "The ~r~biker boys~w~ are showing off a wicked ~g~bike~w~ they just stole. Steal it from them";
                    break;
                case 2:
                    msg = "Apparently the ~r~mexicans~w~ are building an ~g~armored truck~w~ in the desert. Go there and bring it to me.";
                    break;
                case 3:
                    msg = "Nice one my breden. Respect.";
                    break;
                default:
                    msg = "";
                    break;
            }

            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Job", msg);
        }

        void StartMsg()
        {
            switch (_mission)
            {
                case Missions.FM1:
                    ASS_MSG(0);
                    break;
                case Missions.FM2:
                    ASS_MSG(1);
                    break;
                case Missions.FM3:
                    ASS_MSG(2);
                    break;
                case Missions.MM1:
                    REP_MSG(0);
                    break;
                case Missions.MM2:
                    REP_MSG(1);
                    break;
                case Missions.MM3:
                    REP_MSG(2);
                    break;
            }
        }

        void ASS_0()
        {
            GTA.UI.Screen.ShowSubtitle("Go to the ~y~Gang's Location~w~.", 8000);
            Vector3 pos = _locations[0];

            if (!_objtvCrtd)
            {
                _objtvCrtd = true;
                Blip blip = World.CreateBlip(pos);
                blip.Scale = 0.8f;
                blip.Color = BlipColor.Yellow;
                blip.ShowRoute = true;
                blip.Name = "Jacob job objective";
                _blips.Add(blip);
                StartMsg();
            } else
            {
                if (!_routeTA)
                {
                    _routeTA = true;
                    _routeST = Game.GameTime;
                } else if (Game.GameTime - _routeST >= 2000)
                {
                    _routeTA = false;
                    _blips[0].ShowRoute = true;
                }
            }

            if (Game.Player.Character.IsInRange(pos, 225) && !_spawned)
            {
                _spawned = true;

                for (int i = 0; i < _data.ModelTypes.Count; i++)
                {
                    ModelType data1 = _data.ModelTypes[i];
                    Position data2 = _data.Positions[i];
                    int handle;
                    LoadModel(data1.Hash);

                    switch (data1.type)
                    {
                        case 0:
                            handle = Function.Call<int>(Hash.CREATE_PED, 0, data1.Hash, data2.Location.X, data2.Location.Y,
                            data2.Location.Z, 0, false, false);

                            if (_mission == Missions.FM1)
                                Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, handle, _neutral.Hash);
                            else
                                Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, handle, _dislike.Hash);

                            RandomScenario(handle);
                            RandomWeapon(handle);
                            Function.Call(Hash.SET_PED_SUFFERS_CRITICAL_HITS, handle, false);
                            Function.Call(Hash.SET_PED_ARMOUR, handle, 200);
                            Function.Call(Hash.SET_ENTITY_MAX_HEALTH, handle, 200);
                            Function.Call(Hash.SET_ENTITY_HEALTH, handle, 200);
                            Function.Call(Hash.SET_PED_CONFIG_FLAG, handle, 281, true);
                            _peds.Add(handle);
                            break;
                        default:
                            handle = Function.Call<int>(Hash.CREATE_VEHICLE, data1.Hash, data2.Location.X, data2.Location.Y,
                            data2.Location.Z, 0, false, false);
                            _vehicles.Add(handle);
                            break;
                    }

                    FreeModel(data1.Hash);
                    Function.Call(Hash.SET_ENTITY_ROTATION, handle, data2.Rotation.X, data2.Rotation.Y, data2.Rotation.Z, 2, 1);
                }
            } else if (Game.Player.Character.IsInRange(pos, 100) || AnyFightingPlayer())
            {
                for (int i = 0; i < _peds.Count; i++)
                    EnemyBlip(_peds[i]);

                _blips[0].Delete();
                _blips.Clear();
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(1));
                _objective = 1;
            }
        }

        void ASS_1()
        {
            GTA.UI.Screen.ShowSubtitle("Kill the ~r~targets~w~.", 1000);

            if (_peds.Count > 0)
            {
                RemoveDeadEnts();

                if (AnyFightingPlayer() && !_increased)
                {
                    _increased = true;
                    Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(2));
                }

                if (Game.Player.Character.IsShooting && !Function.Call<bool>(Hash.IS_PED_CURRENT_WEAPON_SILENCED, Main.PPID) && !_copsAlerted)
                    _copsAlerted = true;
            } else
            {
                if (!_copsAlerted)
                {
                    Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(0));
                    _objective = 3;
                }
                else
                {
                    Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
                    Game.Player.WantedLevel = 4;
                    _objective = 2;
                }
            }
        }

        void ASS_2()
        {
            GTA.UI.Screen.ShowSubtitle("Lose the cops.", 1000);

            if (Game.Player.WantedLevel > 0)
                return;

            _objective = 3;
        }

        void ASS_3()
        {
            GTA.UI.Screen.ShowSubtitle("Leave the area.", 1000);

            if (!Game.Player.Character.IsInRange(_locations[0], 150))
                Complete();
        }

        void ASS()
        {
            switch(_objective)
            {
                case 0:
                    Game.Player.WantedLevel = 0;
                    ASS_0();
                    break;

                case 1:
                    Game.Player.WantedLevel = 0;
                    ASS_1();
                    break;

                case 2:
                    ASS_2();
                    break;

                case 3:
                    Game.Player.WantedLevel = 0;
                    ASS_3();
                    break;
            }
        }

        void ForceFight()
        {
            for (int i = 0; i < _peds.Count; i++)
            {
                Function.Call(Hash.TASK_COMBAT_PED, _peds[i], Main.PPID, 0, 16);
            }
        }

        string PosText(int code)
        {
            if (_mission == Missions.MM1)
                return code == 0 ? "drug's location" : "drugs";
            else if (_mission == Missions.MM2)
                return code == 0 ? "Sanctus" : "Sanctus";
            else
                return code == 0 ? "Phantom's location" : "Phantom";
        }

        void SpawnChaser(float range)
        {
            OutputArgument outPos = new OutputArgument();
            Vector3 arPPos = Game.Player.Character.Position.Around(range);
            bool result = Function.Call<bool>(Hash.GET_CLOSEST_VEHICLE_NODE, arPPos.X, arPPos.Y, arPPos.Z, outPos, 0, 3, 0);

            if (!result)
                return;

            Vector3 vCoords = outPos.GetResult<Vector3>();
            int v;

            switch(_mission)
            {
                case Missions.MM1:
                    v = Function.Call<int>(Hash.CREATE_VEHICLE, VehicleHash.Chino2, vCoords.X, vCoords.Y, vCoords.Z, 0, false, false);
                    Function.Call(Hash.SET_VEHICLE_MOD_KIT, v, 0);
                    Function.Call(Hash.SET_VEHICLE_COLOURS, v, 145, 145);
                    break;
                case Missions.MM2:
                    v = Function.Call<int>(Hash.CREATE_VEHICLE, VehicleHash.Hexer, vCoords.X, vCoords.Y, vCoords.Z, 0, false, false);
                    break;
                default:
                    v = Function.Call<int>(Hash.CREATE_VEHICLE, VehicleHash.Baller6, vCoords.X, vCoords.Y, vCoords.Z, 0, false, false);
                    break;
            }

            Function.Call(Hash.SET_VEHICLE_ENGINE_ON, v, true, true, false);
            Blip blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, v);
            blip.Scale = 0.85f;
            blip.Sprite = BlipSprite.Enemy;
            blip.Color = BlipColor.Red;
            blip.Name = "Enemy Vehicle";
            int seats = Function.Call<int>(Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, v);

            for (int i = -1; i < seats; i++)
            {
                int ped = Function.Call<int>(Hash.CREATE_PED_INSIDE_VEHICLE, v, 0, _pedModel, i, false, false);
                Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, ped, _dislike);
                Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, WeaponHash.MicroSMG, 2000, false, true);

                if (i == -1)
                {
                    Function.Call(Hash.TASK_VEHICLE_CHASE, ped, Main.PPID);
                    Function.Call(Hash.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG, ped, 1, true);
                }
                else
                {
                    Function.Call(Hash.TASK_VEHICLE_SHOOT_AT_PED, ped, Main.PPID, 40f);
                    Function.Call(Hash.SET_PED_ACCURACY, ped, 33);
                }

                _peds.Add(ped);
            }

            _vehicles.Add(v);
        }

        void REP_0()
        {
            GTA.UI.Screen.ShowSubtitle($"Go to the ~y~{PosText(0)}~w~.", 8000);
            Vector3 pos = _locations[0];

            if (!_objtvCrtd)
            {
                _objtvCrtd = true;
                Blip blip = World.CreateBlip(pos);
                blip.Scale = 0.8f;
                blip.Color = BlipColor.Yellow;
                blip.ShowRoute = true;
                blip.Name = "Jacob job objective";
                _blips.Add(blip);
                StartMsg();
            }
            else
            {
                if (!_routeTA)
                {
                    _routeTA = true;
                    _routeST = Game.GameTime;
                }
                else if (Game.GameTime - _routeST >= 2000)
                {
                    _routeTA = false;
                    _blips[0].ShowRoute = true;
                }
            }

            if (Game.Player.Character.IsInRange(pos, 225) && !_spawned)
            {
                _spawned = true;

                for (int i = 0; i < _data.ModelTypes.Count; i++)
                {
                    ModelType data1 = _data.ModelTypes[i];
                    Position data2 = _data.Positions[i];
                    int handle;
                    LoadModel(data1.Hash);

                    switch (data1.type)
                    {
                        case 0:
                            handle = Function.Call<int>(Hash.CREATE_PED, 0, data1.Hash, data2.Location.X, data2.Location.Y,
                            data2.Location.Z, 0, false, false);

                            if (_mission == Missions.MM3)
                                Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, handle, _dislike.Hash);
                            else
                                Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, handle, _neutral.Hash);

                            RandomScenario(handle);
                            RandomWeapon(handle);
                            Function.Call(Hash.SET_PED_SUFFERS_CRITICAL_HITS, handle, false);
                            Function.Call(Hash.SET_PED_ARMOUR, handle, 200);
                            Function.Call(Hash.SET_ENTITY_MAX_HEALTH, handle, 200);
                            Function.Call(Hash.SET_ENTITY_HEALTH, handle, 200);
                            Function.Call(Hash.SET_PED_CONFIG_FLAG, handle, 281, true);
                            _pedModel = data1.Hash;
                            _peds.Add(handle);
                            break;
                        case 1:
                            handle = Function.Call<int>(Hash.CREATE_VEHICLE, data1.Hash, data2.Location.X, data2.Location.Y,
                                                            data2.Location.Z, 0, false, false);
                            if (_mission == Missions.MM1)
                                _vehicles.Add(handle);
                            else
                                _targetV = handle;

                            break;
                        default:
                            handle = Function.Call<int>(Hash.CREATE_OBJECT_NO_OFFSET, data1.Hash, data2.Location.X,
                                data2.Location.Y, data2.Location.Z, false, false, false);
                            _props.Add(handle);
                            break;
                    }

                    FreeModel(data1.Hash);
                    Function.Call(Hash.SET_ENTITY_ROTATION, handle, data2.Rotation.X, data2.Rotation.Y, data2.Rotation.Z, 2, 1);
                }
            }
            else if (Game.Player.Character.IsInRange(pos, 100) || AnyFightingPlayer())
            {
                for (int i = 0; i < _peds.Count; i++)
                    EnemyBlip(_peds[i]);

                _blips[0].Delete();
                _blips.Clear();

                if (_mission == Missions.MM1)
                {
                    Blip blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, _props[0]);
                    blip.Scale = 0.7f;
                    blip.Color = BlipColor.Green;
                    blip.Name = "Drugs";
                } else
                {
                    Blip blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, _targetV);
                    blip.Scale = 0.7f;
                    blip.Color = BlipColor.Green;

                    if (_mission == Missions.MM2)
                        blip.Name = "Sanctus";
                    else
                        blip.Name = "Phantom";
                }

                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(1));
                _objective = 1;
            }
        }

        void REP_1()
        {
            GTA.UI.Screen.ShowSubtitle($"Steal the ~g~{PosText(1)}~w~.", 1000);
            RemoveDeadEnts();

            if (AnyFightingPlayer() && !_increased)
            {
                _increased = true;
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(2));
            }

            if (_mission == Missions.MM1)
            {
                int prop = _props[0];

                if (!_TookDrugs)
                {
                    if (Game.Player.Character.IsInRange(Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, prop), 1.3f))
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{Main.OpenMenuKey}~ to take the ~g~drugs~w~.", false);

                        if (!_fightFlag)
                        {
                            _fightFlag = true;
                            ForceFight();
                        }

                        if (Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, Main.OpenMenuKey))
                        {
                            _TookDrugs = true;
                            TaskSequence sequence = new TaskSequence();
                            //sequence.AddTask.AchieveHeading(Vector3.RelativeBack.ToHeading());
                            sequence.AddTask.PlayAnimation("pickup_object", "pickup_low");
                            Game.Player.Character.Task.PerformSequence(sequence);
                            Wait(50);
                        }
                    }
                } else
                {
                    if (Game.Player.Character.TaskSequenceProgress == -1)
                    {
                        _TookDrugs = false;
                        Blip blip = World.CreateBlip(_locations[1]);
                        blip.Scale = 0.8f;
                        blip.Color = BlipColor.Yellow;
                        blip.Name = "Drop point";
                        blip.ShowRoute = true;
                        _blips.Add(blip);
                        unsafe
                        {
                            Function.Call(Hash.DELETE_OBJECT, &prop);
                        }
                        _props.Clear();
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Main.PPID, 9, 1, 0, 0);
                        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
                        LoadModel((uint)VehicleHash.Chino2);
                        LoadModel(_pedModel);
                        _objective = 2;
                    }
                }
            } else
            {
                if (Function.Call<bool>(Hash.IS_ENTITY_DEAD, _targetV))
                {
                    RequestScaleform();
                    SetScaleFormText("~r~Mission failed", "Target destroyed");
                    _scaleFormActive = true;
                    _scaleFormStart = Game.GameTime;
                    Quit();
                    return;
                }

                if (Function.Call<int>(Hash.GET_PED_IN_VEHICLE_SEAT, _targetV, -1) == Main.PPID)
                {
                    if (!_fightFlag && _mission != Missions.MM3)
                    {
                        _fightFlag = true;
                        ForceFight();
                    }

                    Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
                    Blip blip = Function.Call<Blip>(Hash.GET_BLIP_FROM_ENTITY, _targetV);
                    blip.Delete();
                    _blipOff = true;
                    if (_mission == Missions.MM2 || _mission == Missions.MM1)
                    {
                        Blip wBlip = World.CreateBlip(_locations[1]);
                        wBlip.Scale = 0.85f;
                        wBlip.Color = BlipColor.Yellow;
                        wBlip.Name = "Drop point";
                        _blips.Add(wBlip);
                        _objective = 2;
                    }
                    else
                    {
                        Game.Player.WantedLevel = 3;
                        _objective = 3;
                    }

                    if (_mission == Missions.MM2)
                        LoadModel((uint)VehicleHash.Hexer);
                    else
                        LoadModel((uint)VehicleHash.Baller6);

                    LoadModel(_pedModel);
                }
            }
        }

        void REP_2()
        {
            GTA.UI.Screen.ShowSubtitle("Go to the ~y~drop point~w~.", 1000);
            Vector3 targetCoords = _locations[1];
            Function.Call(Hash.DRAW_MARKER, 1, targetCoords.X, targetCoords.Y, targetCoords.Z, 0, 0, 0, 0, 0,
                    0, 2f, 2f, 1f, 255, 255, 0, 100, false, false, 2, false, false, false);
            RemoveDeadEnts();
            RemoveFarEnts(270);

            if (!_routeTA)
            {
                _routeTA = true;
                _routeST = Game.GameTime;
            }
            else if (Game.GameTime - _routeST >= 2000)
            {
                _routeTA = false;
                _blips[0].ShowRoute = true;
            }

            if (!Game.Player.Character.IsInRange(_locations[0], 200) && _mission != Missions.MM3)
            {
                if (!Game.Player.Character.IsInRange(targetCoords, 100))
                {
                    if (_vehicles.Count < 3)
                    {
                        if (!_chaserTS)
                        {
                            _chaserTS = true;
                            _chaserST = Game.GameTime;
                        }
                        else if (Game.GameTime - _chaserST >= 5000)
                        {
                            _chaserTS = false;
                            SpawnChaser(_mission == Missions.MM2 ? 70 : 100);
                        }
                    }
                }
            }

            if (_mission == Missions.MM1)
            {
                if (Game.Player.Character.IsInRange(targetCoords, 2) && !Function.Call<bool>(Hash.IS_PED_IN_ANY_VEHICLE, Main.PPID, false))
                {
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Main.PPID, 9, 0, 0, 0);
                    Complete();
                }
            } else
            {
                if (Function.Call<bool>(Hash.IS_ENTITY_DEAD, _targetV))
                {
                    RequestScaleform();
                    SetScaleFormText("~r~Mission failed", "Target destroyed");
                    _scaleFormActive = true;
                    _scaleFormStart = Game.GameTime;
                    Quit();
                    return;
                }

                Vector3 vCoords = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, _targetV);

                if (IsInRange(vCoords, targetCoords, 2))
                {
                    if (Function.Call<int>(Hash.GET_PED_IN_VEHICLE_SEAT, _targetV, -1) == Main.PPID)
                    {
                        Function.Call(Hash.TASK_LEAVE_VEHICLE, Main.PPID, _targetV, 0);
                    }

                    Function.Call(Hash.SET_VEHICLE_DOORS_LOCKED, _targetV, 3);
                    Complete();
                }
                else
                {
                    bool inVehicle = Function.Call<int>(Hash.GET_PED_IN_VEHICLE_SEAT, _targetV, -1) == Main.PPID;

                    if (!inVehicle && _blipOff)
                    {
                        _blipOff = false;
                        Blip blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, _targetV);
                        blip.Scale = 0.7f;
                        blip.Color = BlipColor.Green;

                        if (_mission == Missions.MM2)
                            blip.Name = "Sanctus";
                        else
                            blip.Name = "Phantom";
                    } else if (inVehicle && !_blipOff)
                    {
                        _blipOff = true;
                        Blip blip = Function.Call<Blip>(Hash.GET_BLIP_FROM_ENTITY, _targetV);
                        blip.Delete();
                    }
                }
            }
        }

        void REP_3()
        {
            GTA.UI.Screen.ShowSubtitle("Lose the cops.", 1000);

            if (Game.Player.WantedLevel == 0)
            {
                Blip wBlip = World.CreateBlip(_locations[1]);
                wBlip.Scale = 0.85f;
                wBlip.Color = BlipColor.Yellow;
                wBlip.Name = "Drop point";
                _blips.Add(wBlip);
                _objective = 2;
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(1));
                return;
            }

            if (Function.Call<bool>(Hash.IS_ENTITY_DEAD, _targetV))
            {
                RequestScaleform();
                SetScaleFormText("~r~Mission failed", "Target destroyed");
                _scaleFormActive = true;
                _scaleFormStart = Game.GameTime;
                Quit();
                return;
            }

            Vector3 targetCoords = _locations[1];
            RemoveDeadEnts();
            RemoveFarEnts(270);

            if (!Game.Player.Character.IsInRange(_locations[0], 200))
            {
                if (!Game.Player.Character.IsInRange(targetCoords, 100))
                {
                    if (_vehicles.Count < 3)
                    {
                        if (!_chaserTS)
                        {
                            _chaserTS = true;
                            _chaserST = Game.GameTime;
                        }
                        else if (Game.GameTime - _chaserST >= 5000)
                        {
                            _chaserTS = false;
                            SpawnChaser(80);
                        }
                    }
                }
            }

            bool inVehicle = Function.Call<int>(Hash.GET_PED_IN_VEHICLE_SEAT, _targetV, -1) == Main.PPID;

            if (!inVehicle && _blipOff)
            {
                _blipOff = false;
                Blip blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, _targetV);
                blip.Scale = 0.7f;
                blip.Color = BlipColor.Green;

                if (_mission == Missions.MM2)
                    blip.Name = "Sanctus";
                else
                    blip.Name = "Phantom";
            }
            else if (inVehicle && !_blipOff)
            {
                _blipOff = true;
                Blip blip = Function.Call<Blip>(Hash.GET_BLIP_FROM_ENTITY, _targetV);
                blip.Delete();
            }
        }

        void REP()
        {
            switch (_objective)
            {
                case 0:
                    Game.Player.WantedLevel = 0;
                    REP_0();
                    break;
                case 1:
                    Game.Player.WantedLevel = 0;
                    REP_1();
                    break;
                case 2:
                    Game.Player.WantedLevel = 0;
                    REP_2();
                    break;
                case 3:
                    REP_3();
                    break;
            }
        }

        void OnTick(object sender, EventArgs args)
        {
            if (_scaleFormActive)
            {
                Function.Call(Hash.DRAW_SCALEFORM_MOVIE_FULLSCREEN, _scaleform, 232, 207, 20, 255);

                if (Game.GameTime - _scaleFormStart >= 8000 && !_scaleFormFading)
                {
                    FadeOutScaleform();
                    _scaleFormFading = true;
                }
                else if (Game.GameTime - _scaleFormStart >= 12000)
                {
                    _scaleFormActive = false;
                    _scaleFormFading = false;
                    FreeScaleform();

                    if (_objective == -1)
                        _objective = 0;
                    else if (!_fail)
                        ASS_MSG(3);
                }
            }

            if (!Active)
                return;

            if (_objective == -1)
            {
                if (!_scaleFormActive)
                {
                    ToggleMusicInterrup(true);
                    Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(0));
                    RequestScaleform();
                    SetScaleFormText(MissionTitle(), MissionSubtitle());
                    _scaleFormActive = true;
                    _scaleFormStart = Game.GameTime;
                }

                return;
            }

            if (Game.Player.IsDead)
            {
                Quit();
                return;
            }

            switch (_mission)
            {
                case Missions.FM1:
                case Missions.FM2:
                case Missions.FM3:
                    ASS();
                    break;
                case Missions.MM1:
                case Missions.MM2:
                case Missions.MM3:
                    REP();
                    break;
            }
        }
    }
}
