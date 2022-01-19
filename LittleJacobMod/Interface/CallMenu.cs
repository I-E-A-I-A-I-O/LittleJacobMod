using System;
using System.Collections.Generic;
using LemonUI;
using LemonUI.Menus;
using GTA;
using LittleJacobMod.Loading;

namespace LittleJacobMod.Interface
{
    public class CallMenu
    {
        public static event EventHandler JobSelected;
        public static event EventHandler DeliverySelected;
        public ObjectPool Pool { get; private set; }
        private bool _jobsTimer;
        private bool _deliveryTimer;
        private int _startT;
        private int _startD;
        private NativeItem _jobs;
        private NativeItem _delivery;
        private NativeMenu _mainMenu;
        
        public CallMenu()
        {
            Pool = new ObjectPool();
            _mainMenu = new NativeMenu("Little Jacob", "Contact options");
            _jobs = new NativeItem("Jobs", "Complete jobs for Jacob and earn a especial reward.");
            _delivery = new NativeItem("Delivery", "Earn money delivering product for Jacob.");
            NativeItem shop = new NativeItem("Weapons", "Meet with Jacob to buy weapons from him.");
            _mainMenu.Add(_jobs);
            _mainMenu.Add(_delivery);
            _mainMenu.Add(shop);

            shop.Activated += (o, e) =>
            {
                Hide();
                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "No problem man, just meet me at the location i sent you");
                Main.LittleJacob = Initialize.CalculateClosestSpawnpoint();
                Main.JacobActive = true;
            };

            _jobs.Activated += (o, e) =>
            {
                Hide();
                JobSelected?.Invoke(this, EventArgs.Empty);
            };

            _delivery.Activated += (o, e) =>
            {
                Hide();
                DeliverySelected?.Invoke(this, EventArgs.Empty);
            };

            MissionMain.OnMissionCompleted += (o, e) =>
            {
                _jobs.Enabled = false;
                _jobs.Description = "Next job available in 15:00";
                _jobsTimer = true;
                _startT = Game.GameTime;
            };

            DeliveryMain.OnDeliveryCompleted += (o, e) =>
            {
                _delivery.Enabled = false;
                _delivery.Description = "Next delivery available in 15:00";
                _deliveryTimer = true;
                _startD = Game.GameTime;
            };

            Pool.Add(_mainMenu);
        }

        public void ProcessTimer()
        {
            if (_jobsTimer)
            {
                if (Game.GameTime - _startT >= 900000)
                {
                    _jobsTimer = false;
                    _jobs.Enabled = true;
                    _jobs.Description = "Complete jobs for Jacob and earn a especial reward.";
                }
                else
                {
                    int rem = 900000 - (Game.GameTime - _startT);
                    string des = "Next job available in ";
                    int val = rem / 1000 / 60;

                    if (val < 10)
                        des = string.Concat(des, "0");

                    des = string.Concat(des, $"{val}:");
                    val = rem / 1000 % 60;

                    if (val < 10)
                        des = string.Concat(des, "0");

                    des = string.Concat(des, val.ToString());
                    _jobs.Description = des;
                }
            }

            if (_deliveryTimer)
            {
                if (Game.GameTime - _startD >= 900000)
                {
                    _deliveryTimer = false;
                    _delivery.Enabled = true;
                    _delivery.Description = "Earn money delivering product for Jacob.";
                }
                else
                {
                    int rem = 900000 - (Game.GameTime - _startD);
                    string des = "Next delivery available in ";
                    int val = rem / 1000 / 60;

                    if (val < 10)
                        des = string.Concat(des, "0");

                    des = string.Concat(des, $"{val}:");
                    val = rem / 1000 % 60;

                    if (val < 10)
                        des = string.Concat(des, "0");

                    des = string.Concat(des, val.ToString());
                    _delivery.Description = des;
                }
            }
        }

        public void Show()
        {
            _mainMenu.Visible = true;
        }

        public void Hide()
        {
            Pool.HideAll();
        }
    }
}
