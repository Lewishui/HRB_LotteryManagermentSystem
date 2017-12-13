using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
 

namespace Order.Common
{
    [

       ToolboxBitmapAttribute(typeof(ScrollingText), "ScrollingText.bmp"),

       DefaultEvent("TextClicked")

       ]

    public class ScrollingText : System.Windows.Forms.Control
    {

        private Timer timer;

        private string text = "Text";

        private float staticTextPos = 0;

        private float yPos = 0;

        private ScrollDirection scrollDirection = ScrollDirection.RightToLeft;

        private ScrollDirection currentDirection = ScrollDirection.LeftToRight;

        private VerticleTextPosition verticleTextPosition = VerticleTextPosition.Center;

        private int scrollPixelDistance = 2;

        private bool showBorder = true;

        private bool stopScrollOnMouseOver = false;

        private bool scrollOn = true;

        private Brush foregroundBrush = null;

        private Brush backgroundBrush = null;

        private Color borderColor = Color.Black;

        private RectangleF lastKnownRect;

        public ScrollingText()
        {

            InitializeComponent();

            Version v = System.Environment.Version;

            if (v.Major < 2)
            {

                this.SetStyle(ControlStyles.DoubleBuffer, true);

            }

            else
            {

                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            }

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.SetStyle(ControlStyles.UserPaint, true);

            this.SetStyle(ControlStyles.ResizeRedraw, true);

            timer = new Timer();

            timer.Interval = 25;

            timer.Enabled = true;

            timer.Tick += new EventHandler(Tick);

        }

        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {

                if (foregroundBrush != null)

                    foregroundBrush.Dispose();

                if (backgroundBrush != null)

                    backgroundBrush.Dispose();

                if (timer != null)

                    timer.Dispose();

            }

            base.Dispose(disposing);

        }



        #region Component Designer generated code

        private void InitializeComponent()
        {

            this.Name = "ScrollingText";

            this.Size = new System.Drawing.Size(216, 40);

            this.Click += new System.EventHandler(this.ScrollingText_Click);

        }

        #endregion

        private void Tick(object sender, EventArgs e)
        {

            lastKnownRect.Inflate(10, 5);

            RectangleF refreshRect = lastKnownRect;

            refreshRect.X = Math.Max(0, lastKnownRect.X);

            refreshRect.Width = Math.Min(lastKnownRect.Width + lastKnownRect.X, this.Width);

            refreshRect.Width = Math.Min(this.Width - lastKnownRect.X, refreshRect.Width);

            Region updateRegion = new Region(refreshRect);

            Invalidate(updateRegion);

            Update();

        }

        protected override void OnPaint(PaintEventArgs pe)
        {

            DrawScrollingText(pe.Graphics);

            base.OnPaint(pe);

        }

        public void DrawScrollingText(Graphics canvas)
        {

            canvas.SmoothingMode = SmoothingMode.HighQuality;

            canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;

            SizeF stringSize = canvas.MeasureString(this.text, this.Font);

            if (scrollOn)
            {

                CalcTextPosition(stringSize);

            }

            if (backgroundBrush != null)
            {

                canvas.FillRectangle(backgroundBrush, 0, 0, this.ClientSize.Width, this.ClientSize.Height);

            }

            else
            {

                canvas.Clear(this.BackColor);

            }

            if (showBorder)
            {

                using (Pen borderPen = new Pen(borderColor))

                    canvas.DrawRectangle(borderPen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);

            }

            if (foregroundBrush == null)
            {

                using (Brush tempForeBrush = new System.Drawing.SolidBrush(this.ForeColor))

                    canvas.DrawString(this.text, this.Font, tempForeBrush, staticTextPos, yPos);

            }

            else
            {

                canvas.DrawString(this.text, this.Font, foregroundBrush, staticTextPos, yPos);

            }



            lastKnownRect = new RectangleF(staticTextPos, yPos, stringSize.Width, stringSize.Height);

            EnableTextLink(lastKnownRect);

        }

        private void CalcTextPosition(SizeF stringSize)
        {

            switch (scrollDirection)
            {

                case ScrollDirection.RightToLeft:

                    if (staticTextPos < (-1 * (stringSize.Width)))

                        staticTextPos = this.ClientSize.Width - 1;

                    else

                        staticTextPos -= scrollPixelDistance;

                    break;

                case ScrollDirection.LeftToRight:

                    if (staticTextPos > this.ClientSize.Width)

                        staticTextPos = -1 * stringSize.Width;

                    else

                        staticTextPos += scrollPixelDistance;

                    break;

                case ScrollDirection.Bouncing:

                    if (currentDirection == ScrollDirection.RightToLeft)
                    {

                        if (staticTextPos < 0)

                            currentDirection = ScrollDirection.LeftToRight;

                        else

                            staticTextPos -= scrollPixelDistance;

                    }

                    else if (currentDirection == ScrollDirection.LeftToRight)
                    {

                        if (staticTextPos > this.ClientSize.Width - stringSize.Width)

                            currentDirection = ScrollDirection.RightToLeft;

                        else

                            staticTextPos += scrollPixelDistance;

                    }

                    break;

            }

            switch (verticleTextPosition)
            {

                case VerticleTextPosition.Top:

                    yPos = 2;

                    break;

                case VerticleTextPosition.Center:

                    yPos = (this.ClientSize.Height / 2) - (stringSize.Height / 2);

                    break;

                case VerticleTextPosition.Botom:

                    yPos = this.ClientSize.Height - stringSize.Height;

                    break;

            }

        }

        #region Mouse over, text link logic

        private void EnableTextLink(RectangleF textRect)
        {

            Point curPt = this.PointToClient(Cursor.Position);

            if (textRect.Contains(curPt))
            {

                if (stopScrollOnMouseOver)

                    scrollOn = false;



                this.Cursor = Cursors.Hand;

            }

            else
            {

                scrollOn = true;

                this.Cursor = Cursors.Default;

            }

        }

        private void ScrollingText_Click(object sender, System.EventArgs e)
        {

            if (this.Cursor == Cursors.Hand)

                OnTextClicked(this, new EventArgs());

        }

        public delegate void TextClickEventHandler(object sender, EventArgs args);

        public event TextClickEventHandler TextClicked;

        private void OnTextClicked(object sender, EventArgs args)
        {

            if (TextClicked != null)

                TextClicked(sender, args);

        }

        #endregion

        #region Properties

        [

        Browsable(true),

        CategoryAttribute("Scrolling Text"),

        Description("The timer interval that determines how often the control is repainted")

        ]

        public int TextScrollSpeed
        {

            set
            {

                timer.Interval = value;

            }

            get
            {

                return timer.Interval;

            }

        }

        [

         Browsable(true),

         CategoryAttribute("Scrolling Text"),

         Description("How many pixels will the text be moved per Paint")

         ]

        public int TextScrollDistance
        {

            set
            {

                scrollPixelDistance = value;

            }

            get
            {

                return scrollPixelDistance;

            }

        }



        [

         Browsable(true),

         CategoryAttribute("Scrolling Text"),

         Description("The text that will scroll accros the control")

         ]

        public string ScrollText
        {

            set
            {

                text = value;

                this.Invalidate();

                this.Update();

            }

            get
            {

                return text;

            }

        }

        [

         Browsable(true),

         CategoryAttribute("Scrolling Text"),

         Description("What direction the text will scroll: Left to Right, Right to Left, or Bouncing")

         ]

        public ScrollDirection ScrollDirection
        {

            set
            {

                scrollDirection = value;

            }

            get
            {

                return scrollDirection;

            }

        }

        [

         Browsable(true),

         CategoryAttribute("Scrolling Text"),

         Description("The verticle alignment of the text")

         ]

        public VerticleTextPosition VerticleTextPosition
        {

            set
            {

                verticleTextPosition = value;

            }

            get
            {

                return verticleTextPosition;

            }

        }

        [

         Browsable(true),

         CategoryAttribute("Scrolling Text"),

         Description("Turns the border on or off")

         ]

        public bool ShowBorder
        {

            set
            {

                showBorder = value;

            }

            get
            {

                return showBorder;

            }

        }

        [

         Browsable(true),

         CategoryAttribute("Scrolling Text"),

         Description("The color of the border")

         ]

        public Color BorderColor
        {

            set
            {

                borderColor = value;

            }

            get
            {

                return borderColor;

            }

        }

        [

         Browsable(true),

         CategoryAttribute("Scrolling Text"),

         Description("Determines if the text will stop scrolling if the user's mouse moves over the text")

         ]

        public bool StopScrollOnMouseOver
        {

            set
            {

                stopScrollOnMouseOver = value;

            }

            get
            {

                return stopScrollOnMouseOver;

            }

        }

        [

         Browsable(true),

         CategoryAttribute("Behavior"),

         Description("Indicates whether the control is enabled")

         ]

        new public bool Enabled
        {

            set
            {

                timer.Enabled = value;

                base.Enabled = value;

            }



            get
            {

                return base.Enabled;

            }

        }

        [

         Browsable(false)

         ]

        public Brush ForegroundBrush
        {

            set
            {

                foregroundBrush = value;

            }

            get
            {

                return foregroundBrush;

            }

        }

        [

        ReadOnly(true)

        ]

        public Brush BackgroundBrush
        {

            set
            {

                backgroundBrush = value;

            }

            get
            {

                return backgroundBrush;

            }

        }

        #endregion

    }

    public enum ScrollDirection
    {

        RightToLeft,

        LeftToRight,

        Bouncing

    }

    public enum VerticleTextPosition
    {

        Top,

        Center,

        Botom

    }
}
