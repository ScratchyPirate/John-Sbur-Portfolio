/// <summary>
///  InformationTextbox.cs
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

    public class InformationTextbox : InformationObject
    {
        /// <summary>
        ///  Properties of textbox.
        /// </summary>
        private string text;
        private double fontSize;
        private double height;
        private double width;

        /// <summary>
        ///  Default constuctor. Initializes everything to 0 or empty.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public InformationTextbox()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.Name = InformationObjectConstants.DefaultTextboxName;
            this.Text = InformationObjectConstants.DefaultTextboxText;
            this.FontSize = InformationObjectConstants.DefaultFontSize;
            this.Height = InformationObjectConstants.DefaultHeight;
            this.width = InformationObjectConstants.DefaultWidth;
        }

        /// <summary>
        ///  Constructor that initializes the object based on inputs.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public InformationTextbox(string newName, double newX, double newY, double newFontSize, double newHeight, double newWidth, string newText)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            // Initialize object variables based on inputs.
            this.X = newX;
            this.Y = newY;
            this.Name = newName;
            this.Text = newText;
            this.FontSize = newFontSize;
            this.Height = newHeight;
            this.Width = newWidth;
        }

        /// <summary>
        ///  Setters and Getters.
        /// </summary>
        public string Text
        {
            get { return this.text; }
            set {
                if (value != this.text)
                {
                    this.text = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public double FontSize
        {
            get { return this.fontSize; }
            set {
                if (value != this.fontSize)
                {
                    this.fontSize = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public double Height
        {
            get { return this.height; }
            set
            {
                if (value != this.height)
                {
                    this.height = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public double Width
        {
            get { return this.width; }
            set
            {
                if (value != this.width)
                {
                    this.width = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
    }
}