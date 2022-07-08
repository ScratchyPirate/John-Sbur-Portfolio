/// <summary>
///  InformationCheckbox.cs
///  Job Ticket Manager Project
///  Creator: John D. Sbur
/// </summary>
namespace JobTicketEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class InformationCheckbox : InformationObject
    {
        /// <summary>
        ///  Base length of all checkbox edges without scale.
        /// </summary>
        public static int CheckboxDefaultEdgeLength = 10;

        /// <summary>
        ///  Properties of checkbox objects.
        /// </summary>
        private double scale;
        private bool status;

        /// <summary>
        ///  Default constructor. Sets all relative variables to 0 or false.
        /// </summary>
        public InformationCheckbox()
        {
            this.Name = InformationObjectConstants.DefaultCheckboxName;
            this.Scale = InformationObjectConstants.DefaultScale;
            this.Status = InformationObjectConstants.DefaultCheckboxStatus;
        }

        /// <summary>
        ///  Constructor that initializes the object based on inputs.
        /// </summary>
        public InformationCheckbox(string newName, double newX, double newY, double newScale, bool newStatus)
        {
            // Initialize object variables based on inputs.
            this.X = newX;
            this.Y = newY;
            this.Name = newName;
            this.Scale = newScale;
            this.Status = newStatus;
        }

        /// <summary>
        ///  Setters and Getters.
        /// </summary>
        public double Scale
        {
            get { return scale; }
            set {
                if (value != this.scale)
                {
                    this.scale = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public bool Status
        {
            get { return status; }
            set {
                if (value != this.status)
                {
                    this.status = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
    }
}