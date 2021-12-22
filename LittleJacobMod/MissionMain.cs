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
        RelationshipGroup _hate;
        RelationshipGroup _neutral;
        int _scaleform;
        bool _scaleFormActive;
        int _scaleFormStart;
        bool _scaleFormFading;
        bool _objtvCrtd;
        bool _routeTA;
        int _routeST;
        Random _ran;

        public MissionMain()
        {
            var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
            _dir = $"{BaseDirectory}\\LittleJacobMod\\Missions";
            CancelMissionKey = settings.GetValue("Controls", "CancelMission", Controls.INPUT_SWITCH_VISOR);
            int playerRel = Game.GenerateHash("PLAYER");
            _hate = World.AddRelationshipGroup("JACOB_MISSION_REL_HATE");
            _neutral = World.AddRelationshipGroup("JACOB_MISSION_REL_NEUTRAL");
            _hate.SetRelationshipBetweenGroups(playerRel, Relationship.Hate, true);
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
                    //Active = true;
                    string misName = $"michael_m_{MissionSaving.FProgress}";
                    uint misHash = (uint)Game.GenerateHash(misName);
                    return;
                }
            }

            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Jobs", "I have no jobs for you atm");
        }

        void ResetFlags()
        {
            _spawned = false;
            _increased = false;
            _copsAlerted = false;
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
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(4));
            ToggleMusicInterrup(false);
            Clean();
            ResetFlags();
            Active = false;
        }

        void Quit()
        {
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
                    return "FIXER_MUSIC_STOP";
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

        void RemoveDeadPeds()
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
        }

        void ASS_0()
        {
            GTA.UI.Screen.ShowSubtitle("Go to the ~y~Gang's Location~w~.", 1000);
            Vector3 pos = _locations[0];

            if (!_objtvCrtd)
            {
                _objtvCrtd = true;
                Blip blip = World.CreateBlip(pos);
                blip.Scale = 0.8f;
                blip.Color = BlipColor.Yellow;
                blip.ShowRoute = true;
                _blips.Add(blip);
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

                    if (data1.type == 0)
                    {
                        handle = Function.Call<int>(Hash.CREATE_PED, 0, data1.Hash, data2.Location.X, data2.Location.Y,
                            data2.Location.Z, 0, false, false);
                        Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, handle, _neutral.Hash);
                        RandomScenario(handle);
                        RandomWeapon(handle);
                        Function.Call(Hash.SET_PED_SUFFERS_CRITICAL_HITS, handle, false);
                        Function.Call(Hash.SET_PED_ARMOUR, handle, 250);
                        Function.Call(Hash.SET_ENTITY_MAX_HEALTH, handle, 200);
                        Function.Call(Hash.SET_ENTITY_HEALTH, handle, 200);
                        _peds.Add(handle);
                    }
                    else
                    {
                        handle = Function.Call<int>(Hash.CREATE_VEHICLE, data1.Hash, data2.Location.X, data2.Location.Y,
                            data2.Location.Z, 0, false, false);
                        _vehicles.Add(handle);
                    }

                    FreeModel(data1.Hash);
                    Function.Call(Hash.SET_ENTITY_ROTATION, handle, data2.Rotation.X, data2.Rotation.Y, data2.Rotation.Z, 2, 1);
                }
            } else if (Game.Player.Character.IsInRange(pos, 80))
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
            GTA.UI.Screen.ShowSubtitle("Kill the ~r~targets~w~.", 3000);

            if (_peds.Count > 0)
            {
                RemoveDeadPeds();

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
                    _objective = 3;
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
                    SetScaleFormText(MissionTitle(), "");
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
            }
        }
    }
}
