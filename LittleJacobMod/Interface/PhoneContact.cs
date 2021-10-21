using System;
using System.Collections.Generic;
using iFruitAddon2;
using GTA;
using LittleJacobMod.Loading;

namespace LittleJacobMod.Interface
{
    internal class PhoneContact
    {
        CustomiFruit ifruit;
        public CustomiFruit Phone => ifruit;

        public PhoneContact()
        {
            ifruit = new CustomiFruit();
            var jacobContact = new iFruitContact("Little Jacob")
            {
                DialTimeout = 4000,
                Active = true
            };
            jacobContact.Answered += JacobContact_Answered;
            ifruit.Contacts.Add(jacobContact);
        }

        private void JacobContact_Answered(iFruitContact contact)
        {
            if (Main.JacobActive)
            {
                if (Main.LittleJacob.Spawned)
                {
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "Im busy, call me later");
                } else
                {
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "alrite my brenden. Call me again if you need more weapons");
                    Main.LittleJacob.DeleteBlip();
                    Main.JacobActive = false;
                    Main.TimerStarted = false;
                }
                ifruit.Close();
                return;
            }

            if (Game.Player.WantedLevel > 0)
            {
                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "my friend told me the police is after u. we cant meet like this, call me again when you lose them. Peace");
                ifruit.Close();
                return;
            }

            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "No problem man, just meet me at the location i sent you");
            Main.LittleJacob = Initialize.CalculateClosestSpawnpoint();
            Main.JacobActive = true;

            ifruit.Close();
        }
    }
}
