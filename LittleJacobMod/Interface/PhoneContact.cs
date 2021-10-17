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
            ifruit.Close();

            if (Main.jacobActive)
            {
                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.AllPlayersConf, "Little Jacob", "Meetin", "Breden, i be waitin for u already");
                return;
            }

            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.AllPlayersConf, "Little Jacob", "Meetin", "No problem man, just meet me at the location i sent you");
            Main.LittleJacob = Initialize.CalculateClosestSpawnpoint();
            Main.jacobActive = true;
        }
    }
}
