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
    internal class MissionMain : Script
    {
        private enum Missions : uint
        {
            Fm1 = 274994068,
            Fm2 = 766266916,
            Fm3 = 4044018910,
            Mm1 = 1765589394,
            Mm2 = 1526474001,
            Mm3 = 1290406125
        }

        private readonly List<WeaponHash> _weapons = new()
        {
            WeaponHash.AssaultrifleMk2,
            WeaponHash.PumpShotgunMk2,
            WeaponHash.CombatMGMk2,
            WeaponHash.SMGMk2,
            WeaponHash.DoubleBarrelShotgun,
            WeaponHash.AssaultShotgun
        };

        private struct Position
        {
            public Vector3 Location;
            public Vector3 Rotation;
        }

        private struct ModelType
        {
            public uint Hash;
            public int Type;
        }

        private struct SpawnData
        {
            public List<Position> Positions;
            public List<ModelType> ModelTypes;
        }

        public static bool Active { get; private set; }
        public static event EventHandler OnMissionCompleted;
        private readonly List<int> _peds = new();
        private readonly List<int> _vehicles = new();
        private readonly List<int> _props = new();
        private readonly List<Blip> _blips = new();
        private readonly List<Vector3> _locations = new();
        private SpawnData _data;
        private int _objective;
        private bool _spawned;
        private bool _copsAlerted;
        private bool _increased;
        private readonly string _dir;
        private Missions _mission;
        private Controls _cancelMissionKey;
        private readonly RelationshipGroup _dislike;
        private RelationshipGroup _neutral;
        private int _targetV;
        private bool _chaserTs;
        private int _chaserSt;
        private bool _objtvCrtd;
        private bool _routeTa;
        private bool _tookDrugs;
        private bool _blipOff;
        private bool _fightFlag;
        private bool _fail;
        private int _routeSt;
        private uint _pedModel;
        private readonly Random _ran;

        public MissionMain()
        {
            var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
            _dir = $"{BaseDirectory}\\LittleJacobMod\\Missions";
            _cancelMissionKey = settings.GetValue("Controls", "CancelMission", Controls.INPUT_SWITCH_VISOR);
            var playerRel = Game.GenerateHash("PLAYER");
            _dislike = World.AddRelationshipGroup("JACOB_MISSION_REL_DISLIKE");
            _neutral = World.AddRelationshipGroup("JACOB_MISSION_REL_NEUTRAL");
            _dislike.SetRelationshipBetweenGroups(playerRel, Relationship.Dislike, true);
            _neutral.SetRelationshipBetweenGroups(playerRel, Relationship.Neutral, true);
            _ran = new Random();
            Aborted += MissionMain_Aborted;
            CallMenu.JobSelected += Start;
        }

        private void MissionMain_Aborted(object sender, EventArgs e)
        {
            if (Active)
                Quit();
        }

        private void LoadFromFile(string name)
        {
            var missionData = XElement.Load($"{_dir}\\{name}.xml");
            _data = new SpawnData
            {
                ModelTypes = new List<ModelType>(),
                Positions = new List<Position>()
            };
            var elements = from item in missionData.Elements().Descendants("MapObject")
                          select item;
            var markers = from item in missionData.Elements().Descendants("Marker")
                                             select item;

            for (var i = 0; i < elements.Count(); i++)
            {
                var element = elements.ElementAt(i);
                var pos = new Position();
                var modelType = new ModelType();
                pos.Location.X = (float)element.Element("Position")?.Element("X");
                pos.Location.Y = (float)element.Element("Position")?.Element("Y");
                pos.Location.Z = (float)element.Element("Position")?.Element("Z");
                pos.Rotation.X = (float)element.Element("Rotation")?.Element("X");
                pos.Rotation.Y = (float)element.Element("Rotation")?.Element("Y");
                pos.Rotation.Z = (float)element.Element("Rotation")?.Element("Z");
                modelType.Hash = (uint)(int)element.Element("Hash");
                var type = (string)element.Element("Type");
                
                switch (type)
                {
                    case "Ped":
                        modelType.Type = 0;
                        break;
                    case "Vehicle":
                        modelType.Type = 1;
                        break;
                    case "Prop":
                        modelType.Type = 2;
                        break;
                }

                _data.ModelTypes.Add(modelType);
                _data.Positions.Add(pos);
            }

            var xElements = markers.ToList();
            for (var i = 0; i < xElements.Count(); i++)
            {
                var marker = xElements.ElementAt(i);
                var pos = new Vector3
                {
                    X = (float)marker.Element("Position")?.Element("X"),
                    Y = (float)marker.Element("Position")?.Element("Y"),
                    Z = (float)marker.Element("Position")?.Element("Z")
                };
                _locations.Add(pos);
            }
        }

        private void Start(object o, EventArgs e)
        {
            var model = Function.Call<uint>(Hash.GET_ENTITY_MODEL, Main.PPID);
            
            switch (model)
            {
                case (uint)PedHash.Franklin:
                    {
                        if (MissionSaving.FProgress < 4)
                        {
                            _fail = false;
                            var misName = $"franklin_m_{MissionSaving.FProgress}";
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
                        } else
                        {
                            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Jobs", "I have no jobs for you atm");
                            return;
                        }

                        break;
                    }
                case (uint)PedHash.Michael:
                    {
                        if (MissionSaving.MProgress < 4)
                        {
                            _fail = false;
                            var misName = $"michael_m_{MissionSaving.MProgress}";
                            _mission = (Missions)(uint)Game.GenerateHash(misName);
                            LoadFromFile(misName);

                            if (Game.Player.Character.IsInRange(_locations[0], 250))
                            {
                                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Jobs", "I have no jobs for you atm");
                                Clean();
                                return;
                            }

                            _neutral.SetRelationshipBetweenGroups(Game.GenerateHash("AMBIENT_GANG_LOST"),
                                _mission == Missions.Mm2 ? Relationship.Like : Relationship.Neutral, true);

                            Active = true;
                            _objective = -1;
                        } else
                        {
                            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Jobs", "I have no jobs for you atm");
                            return;
                        }


                        break;
                    }
                case (uint)PedHash.Trevor:
                    {
                        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Jobs", "No.");
                        return;
                    }
                default:
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Jobs", "I have no jobs for you atm");
                    return;
            }

            ToggleMusicInterrup(true);
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(0));
            Main.ShowScaleform(MissionTitle(), MissionSubtitle(), 0);
            Tick += OnTick;
        }

        private void ResetFlags()
        {
            _spawned = false;
            _increased = false;
            _copsAlerted = false;
            _fightFlag = false;
            _objtvCrtd = false;
        }

        private void Complete()
        {
            OnMissionCompleted?.Invoke(this, EventArgs.Empty);
            Game.Player.Money += 25000;
            Main.ShowScaleform("~y~Mission passed", "", 0);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", true);
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(4));
            ToggleMusicInterrup(false);
            Clean();
            ResetFlags();

            switch (_mission)
            {
                case Missions.Fm1:
                    MissionSaving.FProgress += 1;
                    break;
                case Missions.Fm2:
                    MissionSaving.FProgress += 1;
                    break;
                case Missions.Fm3:
                    GTA.UI.Notification.Show("~g~Little Jacob mod~w~: 20% discount unlocked for Franklin");
                    MissionSaving.FProgress += 1;
                    break;
                case Missions.Mm1:
                    MissionSaving.MProgress += 1;
                    break;
                case Missions.Mm2:
                    MissionSaving.MProgress += 1;
                    break;
                case Missions.Mm3:
                    GTA.UI.Notification.Show("~g~Little Jacob mod~w~: 20% discount unlocked for Michael");
                    MissionSaving.MProgress += 1;
                    break;
            }

            MissionSaving.Save();
            ASS_MSG(3);
            Active = false;
            Tick -= OnTick;
        }

        private void Quit()
        {
            _fail = true;
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(5));
            ToggleMusicInterrup(false);
            Clean();
            ResetFlags();
            Active = false;
            Tick -= OnTick;
        }

        private void Clean()
        {
            for (var i = 0; i < _peds.Count; i++)
            {
                var el = _peds[i];
                unsafe
                {
                    Function.Call(Hash.SET_PED_AS_NO_LONGER_NEEDED, &el);
                }
            }

            for (var i = 0; i < _vehicles.Count; i++)
            {
                var el = _vehicles[i];
                unsafe
                {
                    Function.Call(Hash.SET_VEHICLE_AS_NO_LONGER_NEEDED, &el);
                }
            }

            for (var i = 0; i < _props.Count; i++)
            {
                var el = _props[i];
                unsafe
                {
                    Function.Call(Hash.SET_OBJECT_AS_NO_LONGER_NEEDED, &el);
                }
            }

            for (var i = 0; i < _blips.Count; i++)
                _blips[i].Delete();

            _locations.Clear();
            _peds.Clear();
            _vehicles.Clear();
            _props.Clear();
            _blips.Clear();
            _data.ModelTypes.Clear();
            _data.Positions.Clear();

            if ((_mission == Missions.Mm1 || _mission == Missions.Mm2 || _mission == Missions.Mm3) && _objective > 1)
            {
                FreeModel((uint)VehicleHash.Baller6);
                FreeModel(_pedModel);
            }

            if ((_mission == Missions.Mm2 || _mission == Missions.Mm3) && (_objective > 0 || _spawned))
            {
                var v = _targetV;

                unsafe
                {
                    Function.Call(Hash.SET_VEHICLE_AS_NO_LONGER_NEEDED, &v);
                }
            }
        }

        private string MissionTitle()
        {
            switch(_mission)
            {
                case Missions.Fm1:
                case Missions.Fm2:
                case Missions.Fm3:
                    return "~y~Assassination";
                case Missions.Mm1:
                case Missions.Mm2:
                case Missions.Mm3:
                    return "~y~Repossession";
                default:
                    return "";
            }
        }

        private string MissionSubtitle()
        {
            switch (_mission)
            {
                case Missions.Fm1:
                    return "Vagos: Part 1";
                case Missions.Fm2:
                    return "Vagos: Part 2";
                case Missions.Fm3:
                    return "The Lost";
                case Missions.Mm1:
                    return "Drugs";
                case Missions.Mm2:
                    return "Sanctus";
                case Missions.Mm3:
                    return "Phantom";
                default:
                    return "";
            }
        }

        private static void LoadModel(uint model)
        {
            Function.Call(Hash.REQUEST_MODEL, model);

            while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, model))
                Wait(1);
        }

        private static void FreeModel(uint model)
        {
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, model);
        }

        private static string GetEvent(int param)
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

        private static void ToggleMusicInterrup(bool value)
        {
            Function.Call(Hash.SET_AUDIO_FLAG, "DisableFlightMusic", value);
            Function.Call(Hash.SET_AUDIO_FLAG, "WantedMusicDisabled", value);
        }

        private static void EnemyBlip(int ped)
        {
            var blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, ped);
            blip.Scale = 0.7f;
            blip.Sprite = BlipSprite.Enemy;
            blip.Color = BlipColor.Red;
            blip.Name = "Enemy";
        }

        private bool AnyFightingPlayer()
        {
            for (var i = 0; i < _peds.Count; i++)
                if (Function.Call<bool>(Hash.IS_PED_IN_COMBAT, _peds[i], Main.PPID))
                    return true;

            return false;
        }

        private void RandomWeapon(int ped)
        {
            var val = _ran.Next(0, _weapons.Count);
            Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, (uint)_weapons[val], 2000, false, true);
        }

        private void RandomScenario(int ped)
        {
            var val = _ran.Next(0, 3);
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

        private void RemoveFarEnts(float distance)
        {
            for (var i = _peds.Count - 1; i > -1; i--)
            {
                var ped = _peds[i];
                var coords = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, ped);

                if (Game.Player.Character.IsInRange(coords, distance)) continue;
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

                if (Game.Player.Character.IsInRange(coords, distance)) continue;
                unsafe
                {
                    Function.Call(Hash.SET_VEHICLE_AS_NO_LONGER_NEEDED, &v);
                }
                _vehicles.RemoveAt(i);
            }
        }

        private static bool IsInRange(Vector3 a, Vector3 b, float range)
        {
            var nv3 = new Vector3
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
                Z = a.Z - b.Z
            };
            return nv3.X * nv3.X + nv3.Y * nv3.Y + nv3.Z * nv3.Z < range * range;
        }

        private void RemoveDeadEnts()
        {
            for (var i = _peds.Count - 1; i > -1; i--)
            {
                var ped = _peds[i];

                if (!Function.Call<bool>(Hash.IS_PED_DEAD_OR_DYING, ped, 1)) continue;
                unsafe
                {
                    Function.Call(Hash.SET_PED_AS_NO_LONGER_NEEDED, &ped);
                }
                _peds.RemoveAt(i);
            }

            for (var i = _vehicles.Count - 1; i > -1; i--)
            {
                var v = _vehicles[i];

                if (!Function.Call<bool>(Hash.IS_ENTITY_DEAD, v)) continue;
                unsafe
                {
                    Function.Call(Hash.SET_VEHICLE_AS_NO_LONGER_NEEDED, &v);
                }
                _vehicles.RemoveAt(i);
            }
        }

        private static void ASS_MSG(int code)
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

        private static void REP_MSG(int code)
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

        private void StartMsg()
        {
            switch (_mission)
            {
                case Missions.Fm1:
                    ASS_MSG(0);
                    break;
                case Missions.Fm2:
                    ASS_MSG(1);
                    break;
                case Missions.Fm3:
                    ASS_MSG(2);
                    break;
                case Missions.Mm1:
                    REP_MSG(0);
                    break;
                case Missions.Mm2:
                    REP_MSG(1);
                    break;
                case Missions.Mm3:
                    REP_MSG(2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ASS_0()
        {
            GTA.UI.Screen.ShowSubtitle("Go to the ~y~Gang's Location~w~.", 8000);
            var pos = _locations[0];

            if (!_objtvCrtd)
            {
                _objtvCrtd = true;
                var blip = World.CreateBlip(pos);
                blip.Scale = 0.8f;
                blip.Color = BlipColor.Yellow;
                blip.ShowRoute = true;
                blip.Name = "Jacob job objective";
                _blips.Add(blip);
                StartMsg();
            } else
            {
                if (!_routeTa)
                {
                    _routeTa = true;
                    _routeSt = Game.GameTime;
                } else if (Game.GameTime - _routeSt >= 2000)
                {
                    _routeTa = false;
                    _blips[0].ShowRoute = true;
                }
            }

            if (!_spawned)
            {
                _spawned = true;

                for (var i = 0; i < _data.ModelTypes.Count; i++)
                {
                    var data1 = _data.ModelTypes[i];
                    var data2 = _data.Positions[i];
                    int handle;
                    LoadModel(data1.Hash);

                    switch (data1.Type)
                    {
                        case 0:
                            handle = Function.Call<int>(Hash.CREATE_PED, 0, data1.Hash, data2.Location.X, data2.Location.Y,
                            data2.Location.Z, 0, false, false);
                            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, handle, true);

                            Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, handle,
                                _mission == Missions.Fm1 ? _neutral.Hash : _dislike.Hash);

                            RandomScenario(handle);
                            RandomWeapon(handle);
                            Function.Call(Hash.SET_PED_ARMOUR, handle, 250);
                            Function.Call(Hash.SET_ENTITY_MAX_HEALTH, handle, 250);
                            Function.Call(Hash.SET_ENTITY_HEALTH, handle, 250);
                            Function.Call(Hash.SET_PED_CONFIG_FLAG, handle, 281, true);
                            _peds.Add(handle);
                            break;
                        default:
                            handle = Function.Call<int>(Hash.CREATE_VEHICLE, data1.Hash, data2.Location.X, data2.Location.Y,
                            data2.Location.Z, 0, false, false);
                            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, handle, true);
                            _vehicles.Add(handle);
                            break;
                    }

                    FreeModel(data1.Hash);
                    Function.Call(Hash.SET_ENTITY_ROTATION, handle, data2.Rotation.X, data2.Rotation.Y, data2.Rotation.Z, 2, 1);
                }
            } else if (Game.Player.Character.IsInRange(pos, 100) || AnyFightingPlayer())
            {
                for (var i = 0; i < _peds.Count; i++)
                    EnemyBlip(_peds[i]);

                _blips[0].Delete();
                _blips.Clear();
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(1));
                _objective = 1;
            }
        }

        private void ASS_1()
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

        private void ASS_2()
        {
            GTA.UI.Screen.ShowSubtitle("Lose the cops.", 1000);

            if (Game.Player.WantedLevel > 0)
                return;

            _objective = 3;
        }

        private void ASS_3()
        {
            GTA.UI.Screen.ShowSubtitle("Leave the area.", 1000);

            if (!Game.Player.Character.IsInRange(_locations[0], 150))
                Complete();
        }

        private void Ass()
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

        private void ForceFight()
        {
            for (var i = 0; i < _peds.Count; i++)
            {
                Function.Call(Hash.TASK_COMBAT_PED, _peds[i], Main.PPID, 0, 16);
            }
        }

        private string PosText(int code)
        {
            if (_mission == Missions.Mm1)
                return code == 0 ? "drug's location" : "drugs";
            else if (_mission == Missions.Mm2)
                return "Sanctus";
            else
                return code == 0 ? "Phantom's location" : "Phantom";
        }

        private void SpawnChaser(float range)
        {
            var outPos = new OutputArgument();
            var arPPos = Game.Player.Character.Position.Around(range);
            var result = Function.Call<bool>(Hash.GET_CLOSEST_VEHICLE_NODE, arPPos.X, arPPos.Y, arPPos.Z, outPos, 0, 3, 0);

            if (!result)
                return;

            var vCoords = outPos.GetResult<Vector3>();
            int v;

            switch(_mission)
            {
                case Missions.Mm1:
                    v = Function.Call<int>(Hash.CREATE_VEHICLE, (uint)VehicleHash.Chino2, vCoords.X, vCoords.Y, vCoords.Z, 0, false, false);
                    Function.Call(Hash.SET_VEHICLE_MOD_KIT, v, 0);
                    Function.Call(Hash.SET_VEHICLE_COLOURS, v, 145, 145);
                    break;
                case Missions.Mm2:
                    v = Function.Call<int>(Hash.CREATE_VEHICLE, (uint)VehicleHash.Hexer, vCoords.X, vCoords.Y, vCoords.Z, 0, false, false);
                    break;
                default:
                    v = Function.Call<int>(Hash.CREATE_VEHICLE, (uint)VehicleHash.Baller6, vCoords.X, vCoords.Y, vCoords.Z, 0, false, false);
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
                Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, ped, _dislike);
                Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, (uint)WeaponHash.MicroSMG, 2000, false, true);

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

        private void REP_0()
        {
            GTA.UI.Screen.ShowSubtitle($"Go to the ~y~{PosText(0)}~w~.", 8000);
            var pos = _locations[0];

            if (!_objtvCrtd)
            {
                _objtvCrtd = true;
                var blip = World.CreateBlip(pos);
                blip.Scale = 0.8f;
                blip.Color = BlipColor.Yellow;
                blip.ShowRoute = true;
                blip.Name = "Jacob job objective";
                _blips.Add(blip);
                StartMsg();
            }
            else
            {
                if (!_routeTa)
                {
                    _routeTa = true;
                    _routeSt = Game.GameTime;
                }
                else if (Game.GameTime - _routeSt >= 2000)
                {
                    _routeTa = false;
                    _blips[0].ShowRoute = true;
                }
            }

            if (!_spawned)
            {
                _spawned = true;

                for (var i = 0; i < _data.ModelTypes.Count; i++)
                {
                    var data1 = _data.ModelTypes[i];
                    var data2 = _data.Positions[i];
                    int handle;
                    LoadModel(data1.Hash);

                    switch (data1.Type)
                    {
                        case 0:
                            handle = Function.Call<int>(Hash.CREATE_PED, 0, data1.Hash, data2.Location.X, data2.Location.Y,
                            data2.Location.Z, 0, false, false);
                            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, handle, true);

                            Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, handle,
                                _mission == Missions.Mm3 ? _dislike.Hash : _neutral.Hash);

                            RandomScenario(handle);
                            RandomWeapon(handle);
                            Function.Call(Hash.SET_PED_ARMOUR, handle, 250);
                            Function.Call(Hash.SET_ENTITY_MAX_HEALTH, handle, 250);
                            Function.Call(Hash.SET_ENTITY_HEALTH, handle, 200);
                            Function.Call(Hash.SET_PED_CONFIG_FLAG, handle, 281, true);
                            _pedModel = data1.Hash;
                            _peds.Add(handle);
                            break;
                        case 1:
                            handle = Function.Call<int>(Hash.CREATE_VEHICLE, data1.Hash, data2.Location.X, data2.Location.Y,
                                                            data2.Location.Z, 0, false, false);
                            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, handle, true);

                            if (_mission == Missions.Mm1)
                                _vehicles.Add(handle);
                            else
                                _targetV = handle;

                            break;
                        default:
                            handle = Function.Call<int>(Hash.CREATE_OBJECT_NO_OFFSET, data1.Hash, data2.Location.X,
                                data2.Location.Y, data2.Location.Z, false, false, false);
                            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, handle, true);
                            _props.Add(handle);
                            break;
                    }

                    FreeModel(data1.Hash);
                    Function.Call(Hash.SET_ENTITY_ROTATION, handle, data2.Rotation.X, data2.Rotation.Y, data2.Rotation.Z, 2, 1);
                }
            }
            else if (Game.Player.Character.IsInRange(pos, 100) || AnyFightingPlayer())
            {
                for (var i = 0; i < _peds.Count; i++)
                    EnemyBlip(_peds[i]);

                _blips[0].Delete();
                _blips.Clear();

                if (_mission == Missions.Mm1)
                {
                    var blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, _props[0]);
                    blip.Scale = 0.7f;
                    blip.Color = BlipColor.Green;
                    blip.Name = "Drugs";
                } else
                {
                    var blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, _targetV);
                    blip.Scale = 0.7f;
                    blip.Color = BlipColor.Green;
                    blip.Name = _mission == Missions.Mm2 ? "Sanctus" : "Phantom";
                }

                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(1));
                _objective = 1;
            }
        }

        private void REP_1()
        {
            GTA.UI.Screen.ShowSubtitle($"Steal the ~g~{PosText(1)}~w~.", 1000);
            RemoveDeadEnts();

            if (AnyFightingPlayer() && !_increased)
            {
                _increased = true;
                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(2));
            }

            if (_mission == Missions.Mm1)
            {
                var prop = _props[0];

                if (!_tookDrugs)
                {
                    if (!Game.Player.Character.IsInRange(Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, prop), 1.3f))
                        return;
                    GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{Main.OpenMenuKey}~ to take the ~g~drugs~w~.", false);

                    if (!_fightFlag)
                    {
                        _fightFlag = true;
                        ForceFight();
                    }

                    if (!Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, (uint)Main.OpenMenuKey)) return;
                    _tookDrugs = true;
                    var sequence = new TaskSequence();
                    //sequence.AddTask.AchieveHeading(Vector3.RelativeBack.ToHeading());
                    sequence.AddTask.PlayAnimation("pickup_object", "pickup_low");
                    Game.Player.Character.Task.PerformSequence(sequence);
                    Wait(50);
                } else
                {
                    if (Game.Player.Character.TaskSequenceProgress != -1) return;
                    _tookDrugs = false;
                    var blip = World.CreateBlip(_locations[1]);
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

                    if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, (uint)PedHash.Michael))
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Main.PPID, 9, 1, 0, 0);


                    Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
                    LoadModel((uint)VehicleHash.Chino2);
                    LoadModel(_pedModel);
                    _objective = 2;
                }
            } else
            {
                if (Function.Call<bool>(Hash.IS_ENTITY_DEAD, _targetV))
                {
                    Main.ShowScaleform("~r~Mission failed", "Target destroyed", 0);
                    Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
                    Quit();
                    return;
                }

                if (Function.Call<int>(Hash.GET_PED_IN_VEHICLE_SEAT, _targetV, -1) != Main.PPID) return;
                if (!_fightFlag && _mission != Missions.Mm3)
                {
                    _fightFlag = true;
                    ForceFight();
                }

                Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(3));
                var blip = Function.Call<Blip>(Hash.GET_BLIP_FROM_ENTITY, _targetV);
                blip.Delete();
                _blipOff = true;
                if (_mission == Missions.Mm2 || _mission == Missions.Mm1)
                {
                    var wBlip = World.CreateBlip(_locations[1]);
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

                if (_mission == Missions.Mm2)
                    LoadModel((uint)VehicleHash.Hexer);
                else
                    LoadModel((uint)VehicleHash.Baller6);

                LoadModel(_pedModel);
            }
        }

        private void REP_2()
        {
            GTA.UI.Screen.ShowSubtitle("Go to the ~y~drop point~w~.", 1000);
            var targetCoords = _locations[1];
            Function.Call(Hash.DRAW_MARKER, 1, targetCoords.X, targetCoords.Y, targetCoords.Z, 0, 0, 0, 0, 0,
                    0, 2f, 2f, 1f, 255, 255, 0, 100, false, false, 2, false, false, false);
            RemoveDeadEnts();
            RemoveFarEnts(270);

            if (!_routeTa)
            {
                _routeTa = true;
                _routeSt = Game.GameTime;
            }
            else if (Game.GameTime - _routeSt >= 2000)
            {
                _routeTa = false;
                _blips[0].ShowRoute = true;
            }

            if (!Game.Player.Character.IsInRange(_locations[0], 200) && _mission != Missions.Mm3)
            {
                if (!Game.Player.Character.IsInRange(targetCoords, 100))
                {
                    if (_vehicles.Count < 3)
                    {
                        if (!_chaserTs)
                        {
                            _chaserTs = true;
                            _chaserSt = Game.GameTime;
                        }
                        else if (Game.GameTime - _chaserSt >= 5000)
                        {
                            _chaserTs = false;
                            SpawnChaser(_mission == Missions.Mm2 ? 70 : 100);
                        }
                    }
                }
            }

            if (_mission == Missions.Mm1)
            {
                if (!Game.Player.Character.IsInRange(targetCoords, 2) ||
                    Function.Call<bool>(Hash.IS_PED_IN_ANY_VEHICLE, Main.PPID, false)) return;
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Main.PPID, 9, 0, 0, 0);
                Complete();
            } else
            {
                if (Function.Call<bool>(Hash.IS_ENTITY_DEAD, _targetV))
                {
                    Main.ShowScaleform("~r~Mission failed", "Target destroyed", 0);
                    Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
                    Quit();
                    return;
                }

                var vCoords = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, _targetV);

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
                    var inVehicle = Function.Call<int>(Hash.GET_PED_IN_VEHICLE_SEAT, _targetV, -1) == Main.PPID;

                    switch (inVehicle)
                    {
                        case false when _blipOff:
                        {
                            _blipOff = false;
                            var blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, _targetV);
                            blip.Scale = 0.7f;
                            blip.Color = BlipColor.Green;
                            blip.Name = _mission == Missions.Mm2 ? "Sanctus" : "Phantom";
                            break;
                        }
                        case true when !_blipOff:
                        {
                            _blipOff = true;
                            var blip = Function.Call<Blip>(Hash.GET_BLIP_FROM_ENTITY, _targetV);
                            blip.Delete();
                            break;
                        }
                    }
                }
            }
        }

        private void REP_3()
        {
            GTA.UI.Screen.ShowSubtitle("Lose the cops.", 1000);

            if (Game.Player.WantedLevel == 0)
            {
                var wBlip = World.CreateBlip(_locations[1]);
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
                Main.ShowScaleform("~r~Mission failed", "Target destroyed", 0);
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
                Quit();
                return;
            }

            var targetCoords = _locations[1];
            RemoveDeadEnts();
            RemoveFarEnts(270);

            if (!Game.Player.Character.IsInRange(_locations[0], 200))
            {
                if (!Game.Player.Character.IsInRange(targetCoords, 100))
                {
                    if (_vehicles.Count < 3)
                    {
                        if (!_chaserTs)
                        {
                            _chaserTs = true;
                            _chaserSt = Game.GameTime;
                        }
                        else if (Game.GameTime - _chaserSt >= 5000)
                        {
                            _chaserTs = false;
                            SpawnChaser(80);
                        }
                    }
                }
            }

            var inVehicle = Function.Call<int>(Hash.GET_PED_IN_VEHICLE_SEAT, _targetV, -1) == Main.PPID;

            switch (inVehicle)
            {
                case false when _blipOff:
                {
                    _blipOff = false;
                    var blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, _targetV);
                    blip.Scale = 0.7f;
                    blip.Color = BlipColor.Green;
                    blip.Name = _mission == Missions.Mm2 ? "Sanctus" : "Phantom";
                    break;
                }
                case true when !_blipOff:
                {
                    _blipOff = true;
                    var blip = Function.Call<Blip>(Hash.GET_BLIP_FROM_ENTITY, _targetV);
                    blip.Delete();
                    break;
                }
            }
        }

        private void Rep()
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

        private void OnTick(object sender, EventArgs args)
        {
            if (!Active)
                return;

            if (_objective == -1)
            {
                if (!Main.ScaleformActive)
                {
                    _objective = 0;
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
                case Missions.Fm1:
                case Missions.Fm2:
                case Missions.Fm3:
                    Ass();
                    break;
                case Missions.Mm1:
                case Missions.Mm2:
                case Missions.Mm3:
                    Rep();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
