namespace Poker
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Player : INotifyPropertyChanged
    {
        private const string PlayerNameMe = "Me";

        /// <summary>
        /// Required by the INotifyPropertyChanged interface.  Calling this will
        /// update property values in data grid for example.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private float chipCount = 0.0f;
        private string playerName = string.Empty;
        private int position = 1;
        private bool folded = true;

        public Player(string playerNameIn, float chipCountIn)
        {
            if (string.IsNullOrWhiteSpace(playerNameIn))
            {
                playerNameIn = PlayerNameMe;
            }

            this.PlayerName = playerNameIn;
            this.ChipCount = chipCountIn;
        }

        public string PlayerName
        {
            get
            {
                return this.playerName;
            }

            private set
            {
                this.playerName = value;
                this.OnPropertyChanged("PlayerName");
            }
        }
        
        public float ChipCount
        {
            get
            {
                return this.chipCount;
            }

            private set
            {
                this.chipCount = value;
                this.OnPropertyChanged("ChipCount");
            }
        }

        public int Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.position = value;
                this.OnPropertyChanged("Position");
            }
        }

        public bool Folded
        {
            get
            {
                return this.folded;
            }

            set
            {
                this.folded = value;
                this.OnPropertyChanged("Folded");
            }
        }

        public static Player FromString(string dataString)
        {
            string[] split = dataString.Split('|');
            if (split.Length != 2)
            {
                return null;
            }

            float chipCount;
            if (!float.TryParse(split[1], out chipCount))
            {
                return null;
            }

            Player result = new Player(split[0], chipCount);
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", this.PlayerName, this.ChipCount);
        }

        /// <summary>
        /// Will update the value of a tracked property.  The handler
        /// required by INotifyPropertyChanged interface will
        /// have a data grid update its values, for example.
        /// </summary>
        /// <param name="propertyName">The name of the property to update.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
