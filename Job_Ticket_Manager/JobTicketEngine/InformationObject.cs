/// <summary>
///  InformationObject.cs
///  Job Ticket Manager Project
///  Creator: John D. Sbur
/// </summary>
namespace JobTicketEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///  The class responsible for holding information pertaining to job tickets and templates. Meant to set a structure for other classes to inherit from.
    ///  Classes inheriting have the structure such that they can be added to a job ticket and a template created by the user.
    /// </summary>
    public class InformationObject
    {
        /// <summary>
        ///  Position variables for when the object is placed on a document.
        /// </summary>
        private double x;
        private double y;

        /// <summary>
        ///  Title of the object.
        /// </summary>
        private string name;

        /// <summary>
        ///  For used outside of the data object. If information is required to be inside this object, this is set to true. Otherwise, it is set to false.
        /// </summary>
        private bool required;

        /// <summary>
        ///  This variable corresponds with when the object will appear in the creation window. 
        ///  Low priority implies it will show up sooner and higher priority will show up later.
        /// </summary>
        private int priority;

        /// <summary>
        ///  Used for signalling when a property of the information object is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///  Default constructor. Initiates position to 0,0 and name to "default"
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public InformationObject()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.X = InformationObjectConstants.DefaultX;
            this.Y = InformationObjectConstants.DefaultY;
            this.Name = InformationObjectConstants.DefaultName;
        }

        /// <summary>
        ///  Constructor that initializes the object based on inputs.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public InformationObject(string newName, double newX, double newY)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.X = newX;
            this.Y = newY;
            this.Name = newName;
            this.Priority = 0;
        }

        /// <summary>
        ///  Setters and Getters.
        /// </summary>
        public double X
        {
            get { return this.x; }
            set {
                if (value != this.x)
                {
                    this.x = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public double Y
        {
            get { return this.y; }
            set
            {
                if (value != this.y)
                {
                    this.y = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public string Name
        {
            get { return this.name; }
            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public bool Required
        {
            get { return this.required; }
            set
            {
                if (value != this.required)
                {
                    this.required = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public int Priority
        {
            get { return this.priority; }
            set
            {
                if (value != this.priority)
                {
                    this.priority = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        ///  Changes the value of priority without triggering a property changed event.
        /// </summary>
        public int SafePriority
        {
            get
            {
                return this.priority;
            }
            set
            {
                this.priority = value;
            }
        }
        /// <summary>
        ///  Notifies subscribers that a property of the information object has changed.
        /// </summary>
        protected void NotifyPropertyChanged()
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("base information property"));
        }
    }
}