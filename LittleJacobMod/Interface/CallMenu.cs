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
        public ObjectPool Pool { get; private set; }
        NativeMenu _mainMenu;
        
        public CallMenu()
        {
            Pool = new ObjectPool();
            _mainMenu = new NativeMenu("Little Jacob", "Contact options");
            NativeItem mission = new NativeItem("Jobs", "Complete jobs for Jacob and earn a especial reward.");
            NativeItem shop = new NativeItem("Weapons", "Meet with Jacob to buy weapons from him.");
            _mainMenu.Add(mission);
            _mainMenu.Add(shop);

            shop.Activated += (o, e) =>
            {
                Hide();
                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "No problem man, just meet me at the location i sent you");
                Main.LittleJacob = Initialize.CalculateClosestSpawnpoint();
                Main.JacobActive = true;
            };

            mission.Activated += (o, e) =>
            {
                Hide();
                MissionMain.Start();
            };

            Pool.Add(_mainMenu);
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
