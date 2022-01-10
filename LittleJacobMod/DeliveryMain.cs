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
    int _cop;
    Blip _routeBlip;
    Ped _buyer;
    Vector3 _destination;
    Vehicle _car;
    int _objective;

    public DeliveryMain()
    {
        _neutral = World.AddRelationshipGroup("DRUG_DELIVERY_NEUTRAL_REL");
        _cop = Game.GenerateHash("COP");
        int player = Game.GenerateHash("PLAYER");
        _neutral.SetRelationshipBetweenGroups(player, Relationship.Neutral, true);
        _neutral.SetRelationshipBetweenGroups(_cop, Relationship.Neutral, true);
        CallMenu.DeliverySelected += Start;
        Aborted += DeliveryMain_Aborted;
    }

    private void DeliveryMain_Aborted(object sender, EventArgs e)
    {
        if (Active)
        {
            _car.MarkAsNoLongerNeeded();
            _buyer.MarkAsNoLongerNeeded();
        }
    }

    void RequestModel(int model)
    {
        Function.Call(Hash.REQUEST_MODEL, model);

        while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, model))
            Wait(1);
    }

    string GetEvent(int param)
    {
        switch (param)
        {
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

        for (int i = 0; i < size; i++)
        {
            XElement el = buyers.ElementAt(i);
            XElement buyer = el.Element("Buyer");
            XElement marker = el.Element("Marker");
            Vector3 markerPoint = new Vector3();
            markerPoint.X = (float)marker.Element("X");
            markerPoint.Y = (float)marker.Element("Y");
            markerPoint.Z = (float)marker.Element("Z");
            float dist = World.CalculateTravelDistance(Game.Player.Character.Position, markerPoint);

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

        int index = ran.Next(0, size);
        HeadingPoint selectedBuyer = validBuyers.ElementAt(index);
        RequestModel(selectedBuyer.Hash);
        _buyer = Function.Call<Ped>(Hash.CREATE_PED, 0, selectedBuyer.Hash, selectedBuyer.X, selectedBuyer.Y,
            selectedBuyer.Z, selectedBuyer.Heading, false, false);
        Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, _buyer.Handle, true);
        _buyer.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
        _buyer.RelationshipGroup = _neutral;
        _buyer.Weapons.Give(WeaponHash.Pistol, 100, false, true);
        _destination = markers.ElementAt(index);
        _car = Function.Call<Vehicle>(Hash.CREATE_VEHICLE, VehicleHash.Tornado3, carPoint.X, carPoint.Y,
            carPoint.Z, carPoint.Heading, false, false);
        _car.AddBlip();
        _car.AttachedBlip.Scale = 0.8f;
        _car.AttachedBlip.Sprite = BlipSprite.Weed;
        _car.AttachedBlip.Color = BlipColor.Green;
        Main.ShowScaleform("~g~Weed Delivery", "", 0);
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, GetEvent(0));
        _objective = -1;
        Tick += DeliveryMain_Tick;
        Active = true;
    }

    void DEL_0()
    {
        GTA.UI.Screen.ShowSubtitle("Get in the ~g~car.", 1000);

        if (!Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Main.PPID, _car.Handle, false))
            return;

        _car.AttachedBlip.Delete();
        _objective = 1;
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

        switch(_objective)
        {
            case 0:
                DEL_0();
                break;
        }
    }
}
