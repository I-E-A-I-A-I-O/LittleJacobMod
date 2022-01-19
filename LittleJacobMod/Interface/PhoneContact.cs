using iFruitAddon2;
using GTA;

namespace LittleJacobMod.Interface
{
    internal class PhoneContact
    {
        public CustomiFruit Phone { get; }

        public PhoneContact()
        {
            Phone = new CustomiFruit();
            var jacobContact = new iFruitContact("Little Jacob")
            {
                DialTimeout = 4000,
                Active = true
            };
            jacobContact.Answered += JacobContact_Answered;
            Phone.Contacts.Add(jacobContact);
        }

        private void JacobContact_Answered(iFruitContact contact)
        {
            if (MissionMain.Active || DeliveryMain.Active)
            {
                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Busy", "Im busy, call me later");
                return;
            }

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
                Phone.Close();
                return;
            }

            if (Game.Player.WantedLevel > 0)
            {
                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "my friend told me the police is after u. we cant meet like this, call me again when you lose them. Peace");
                Phone.Close();
                return;
            }

            Main.CallMenu.Show();
            Phone.Close();
        }
    }
}
