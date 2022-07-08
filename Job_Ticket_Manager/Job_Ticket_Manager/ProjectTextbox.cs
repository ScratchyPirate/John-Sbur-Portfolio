/// <summary>
///  ProjectTextbox.cs
///  Job Ticket Manager Project
///  Creator: John D. Sbur
/// </summary>
namespace Job_Ticket_Manager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProjectTextbox : TextBox
    {
        /// <summary>
        ///  Helps keep track of the maximum number of characters that can be entered into a textbox.
        /// </summary>
        private int maxCharacters;

        /// <summary>
        ///  Holder variable for user use.
        /// </summary>
        private string? textStorage;

        /// <summary>
        ///  A unique id given to this textbox.
        /// </summary>
        private int _ID;

        /// <summary>
        /// Keeps track of line count at user's request.
        /// </summary>
        private int previousLineCount;
        
        /// <summary>
        ///  Keeps track of where new lines are located. Interacted with my the user.
        /// </summary>
        private List<int> newLineAt;

        /// <summary>
        ///  Default constructor.
        /// </summary>
        public ProjectTextbox()
        {
            this.newLineAt = new List<int>();
            this.newLineAt.Clear();
            this.maxCharacters = this.CalculateMaxCharacters();
            this.previousLineCount = 1;
            this.Text = string.Empty;
        }

        /// <summary>
        ///  Calculates the maximum number of characters this textbox can hold. It's calculated from
        ///  the textbox's height, width, and font size.
        ///  Fontsize Height = Height of E gamma character
        ///  Font width is also E gamma in this case for simplicity.
        /// </summary>
        /// <returns></returns>
        public int CalculateMaxCharacters()
        {
            int calculatedMax = 0;
            using (Graphics g = Graphics.FromImage(new Bitmap(this.Width, this.Height)))
            {
                calculatedMax = (int)((this.Width) / (g.MeasureString("X", this.Font).Width)) * (int)(this.Height / g.MeasureString("X", this.Font).Height);
            }

            // If the calculated Max is less than 1, we need to encure that the max is at least 1
            if (calculatedMax < 1)
            {
                calculatedMax = 1;
            }

            return calculatedMax;
        }

        /// <summary>
        ///  Updates the maximum number of characters this textbox can hold.
        /// </summary>
        private void UpdateMaxCharacters()
        {
            this.maxCharacters = this.CalculateMaxCharacters();
        }

        /// <summary>
        ///  Gets the maximum number of characters that can be entered into a textbox.
        /// </summary>
        public int MaxCharacters
        {
            get { return this.maxCharacters; }
        }

        /// <summary>
        ///  Used for storing values within the textbox to be pulled later.
        /// </summary>
        public string? TextStorage
        {
            get { return this.textStorage; }
            set { this.textStorage = value; }
        }

        /// <summary>
        ///  Gets or Sets this textbox's ID.
        /// </summary>
        public int ID 
        {
            get { return this._ID; }
            set { this._ID = value; }
        }

        /// <summary>
        ///  Gets or Sets font of this textbox. Updates max number of characters to match new font.
        /// </summary>
        public Font ProjectFont
        {
            get { return this.Font; }
            set
            {
                this.Font = value;
                this.UpdateMaxCharacters();
            }
        }

        /// <summary>
        ///  Gets or Sets previous line count.
        /// </summary>
        public int PreviousLineCount
        {
            get { return this.previousLineCount; }
            set { this.previousLineCount = value; }
        }

        public List<int> NewLineAt
        {
            get { return this.newLineAt; }
            set { this.newLineAt = value; }
        }
    }
}