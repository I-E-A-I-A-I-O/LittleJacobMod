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
        public ObjectPool Pool { get; private set; }
        public bool TimerActive { get; private set; }
        int _startT;
        NativeItem _jobs;
        NativeMenu _mainMenu;
        
        public CallMenu()
        {
            Pool = new ObjectPool();
            _mainMenu = new NativeMenu("Little Jacob", "Contact options");
            _jobs = new NativeItem("Jobs", "Complete jobs for Jacob and earn a especial reward.");
            NativeItem shop = new NativeItem("Weapons", "Meet with Jacob to buy weapons from him.");
            _mainMenu.Add(_jobs);
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

            MissionMain.OnMissionCompleted += (o, e) =>
            {
                _jobs.Enabled = false;
                _jobs.Description = "Next job available in 15:00";
                TimerActive = true;
                _startT = Game.GameTime;
            };

            Pool.Add(_mainMenu);
        }

        public void ProcessTimer()
        {
            if (Game.GameTime - _startT >= 900000)
            {
                TimerActive = false;
                _jobs.Enabled = true;
                _jobs.Description = "Complete jobs for Jacob and earn a especial reward.";
            } else
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
