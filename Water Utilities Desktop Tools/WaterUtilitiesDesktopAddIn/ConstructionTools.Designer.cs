using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace A4WaterUtilities
{
    public partial class AddLateralsConstructionTool
    {
        #region Tool input overriding methods
        protected sealed override void OnMouseDown(MouseEventArgs arg)
        {
            if (m_csc != null)
            m_csc.OnMouseDown(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseMove(MouseEventArgs arg)
        {
            if (m_csc !=  null)
                m_csc.OnMouseMove(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseUp(MouseEventArgs arg)
        {
            if (m_csc != null)
            m_csc.OnMouseUp(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnKeyDown(KeyEventArgs arg)
        {
            if (m_csc != null)
            m_csc.OnKeyDown((int)arg.KeyCode, keyshift2int(arg));
        }
        protected sealed override void OnKeyUp(KeyEventArgs arg)
        {
            if (m_csc != null)
            m_csc.OnKeyUp((int)arg.KeyCode, keyshift2int(arg));
        }

        protected sealed override bool OnContextMenu(int x, int y)
        {
            if (m_csc != null)
                return m_csc.OnContextMenu(x, y);
            else return false;
        }

        protected sealed override void OnRefresh(int hDC)
        {
            if (m_csc != null)
                m_csc.Refresh(hDC);
        }

        //int translations
        private int mousebutton2int(MouseEventArgs arg)
        {
            switch (arg.Button)
            {
                case MouseButtons.Left: return 1;
                case MouseButtons.Right: return 2;
                case MouseButtons.Left | MouseButtons.Right: return 3;
                case MouseButtons.Middle: return 4;
                case MouseButtons.Left | MouseButtons.Middle: return 5;
                case MouseButtons.Middle | MouseButtons.Right: return 6;
                case MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right: return 7;
                default: return 0;
            }
        }

        private int mouseshift2int(MouseEventArgs arg)
        {
            int shift = (arg.Shift ? 1 : 0) +
            (arg.Control ? 2 : 0) +
            (arg.Alt ? 4 : 0);
            return shift;
        }

        private int keyshift2int(KeyEventArgs arg)
        {
            switch (arg.KeyCode)
            {
                case Keys.None: return 0;
                case Keys.Shift: return 1;
                case Keys.Control: return 2;
                case Keys.Shift | Keys.Control: return 3;
                case Keys.Alt: return 4;
                case Keys.Shift | Keys.Alt: return 5;
                case Keys.Control | Keys.Alt: return 6;
                case Keys.Shift | Keys.Control | Keys.Alt: return 7;
                default: return 0;
            }
        }
        #endregion

        #region ISketchTool Members
        void ISketchTool.AddPoint(IPoint point, bool Clone, bool allowUndo)
        {
            if (m_csc != null)
                m_csc.AddPoint(point, Clone, allowUndo);
        }

        IPoint ISketchTool.Anchor
        {
            get {
                if (m_csc != null)
                    return m_csc.Anchor;
                else
                    return null;

            }
        }

        double ISketchTool.AngleConstraint
        {
            get {
                if (m_csc != null)
                    return m_csc.AngleConstraint;
                else
                    return 0.0;
            }
            set { m_csc.AngleConstraint = value; }
        }

        esriSketchConstraint ISketchTool.Constraint
        {
            get {
                if (m_csc != null)
                    return m_csc.Constraint;
                else
                    return esriSketchConstraint.esriConstraintNone;

            }
            set { m_csc.Constraint = value; }
        }

        double ISketchTool.DistanceConstraint
        {
            get {
                if (m_csc != null)
                    return m_csc.DistanceConstraint;
                else
                    return 0.0;

            }
            set { m_csc.DistanceConstraint = value; }
        }

        bool ISketchTool.IsStreaming
        {
            get {
                if (m_csc != null)
                    return m_csc.IsStreaming;
                else
                    return false;

            }
            set { m_csc.IsStreaming = value; }
        }

        IPoint ISketchTool.Location
        {
            get {
                if (m_csc != null)
                    return m_csc.Location;
                else
                    return null;

            }
        }
        #endregion
    }
    public partial class AddLateralsFromMainPointConstructionTool
    {
        #region Tool input overriding methods
        protected sealed override void OnMouseDown(MouseEventArgs arg)
        {
            if (m_csc != null)
                m_csc.OnMouseDown(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseMove(MouseEventArgs arg)
        {
            if (m_csc != null)
                m_csc.OnMouseMove(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseUp(MouseEventArgs arg)
        {
            if (m_csc != null)
                m_csc.OnMouseUp(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnKeyDown(KeyEventArgs arg)
        {
            if (m_csc != null)
                m_csc.OnKeyDown((int)arg.KeyCode, keyshift2int(arg));
        }
        protected sealed override void OnKeyUp(KeyEventArgs arg)
        {
            if (m_csc != null)
                m_csc.OnKeyUp((int)arg.KeyCode, keyshift2int(arg));
        }

        protected sealed override bool OnContextMenu(int x, int y)
        {
            if (m_csc != null)
                return m_csc.OnContextMenu(x, y);
            else return false;
        }

        protected sealed override void OnRefresh(int hDC)
        {
            if (m_csc != null)
                m_csc.Refresh(hDC);
        }

        //int translations
        private int mousebutton2int(MouseEventArgs arg)
        {
            switch (arg.Button)
            {
                case MouseButtons.Left: return 1;
                case MouseButtons.Right: return 2;
                case MouseButtons.Left | MouseButtons.Right: return 3;
                case MouseButtons.Middle: return 4;
                case MouseButtons.Left | MouseButtons.Middle: return 5;
                case MouseButtons.Middle | MouseButtons.Right: return 6;
                case MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right: return 7;
                default: return 0;
            }
        }

        private int mouseshift2int(MouseEventArgs arg)
        {
            int shift = (arg.Shift ? 1 : 0) +
            (arg.Control ? 2 : 0) +
            (arg.Alt ? 4 : 0);
            return shift;
        }

        private int keyshift2int(KeyEventArgs arg)
        {
            switch (arg.KeyCode)
            {
                case Keys.None: return 0;
                case Keys.Shift: return 1;
                case Keys.Control: return 2;
                case Keys.Shift | Keys.Control: return 3;
                case Keys.Alt: return 4;
                case Keys.Shift | Keys.Alt: return 5;
                case Keys.Control | Keys.Alt: return 6;
                case Keys.Shift | Keys.Control | Keys.Alt: return 7;
                default: return 0;
            }
        }
        #endregion

        #region ISketchTool Members
        void ISketchTool.AddPoint(IPoint point, bool Clone, bool allowUndo)
        {
            if (m_csc != null)
                m_csc.AddPoint(point, Clone, allowUndo);
        }

        IPoint ISketchTool.Anchor
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Anchor;
                else
                    return null;

            }
        }

        double ISketchTool.AngleConstraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.AngleConstraint;
                else
                    return 0.0;
            }
            set { m_csc.AngleConstraint = value; }
        }

        esriSketchConstraint ISketchTool.Constraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Constraint;
                else
                    return esriSketchConstraint.esriConstraintNone;

            }
            set { m_csc.Constraint = value; }
        }

        double ISketchTool.DistanceConstraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.DistanceConstraint;
                else
                    return 0.0;

            }
            set { m_csc.DistanceConstraint = value; }
        }

        bool ISketchTool.IsStreaming
        {
            get
            {
                if (m_csc != null)
                    return m_csc.IsStreaming;
                else
                    return false;

            }
            set { m_csc.IsStreaming = value; }
        }

        IPoint ISketchTool.Location
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Location;
                else
                    return null;

            }
        }
        #endregion
    }
    public partial class AddLineWithEndPoints
    {
        #region Tool input overriding methods
        protected sealed override void OnMouseDown(MouseEventArgs arg)
        {
            m_csc.OnMouseDown(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseMove(MouseEventArgs arg)
        {
            m_csc.OnMouseMove(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseUp(MouseEventArgs arg)
        {
            m_csc.OnMouseUp(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnKeyDown(KeyEventArgs arg)
        {
            m_csc.OnKeyDown((int)arg.KeyCode, keyshift2int(arg));
        }
        protected sealed override void OnKeyUp(KeyEventArgs arg)
        {
            m_csc.OnKeyUp((int)arg.KeyCode, keyshift2int(arg));
        }

        protected sealed override bool OnContextMenu(int x, int y)
        {
            return m_csc.OnContextMenu(x, y);
        }

        protected sealed override void OnRefresh(int hDC)
        {
            if (m_csc != null)
                m_csc.Refresh(hDC);
        }

        //int translations
        private int mousebutton2int(MouseEventArgs arg)
        {
            switch (arg.Button)
            {
                case MouseButtons.Left: return 1;
                case MouseButtons.Right: return 2;
                case MouseButtons.Left | MouseButtons.Right: return 3;
                case MouseButtons.Middle: return 4;
                case MouseButtons.Left | MouseButtons.Middle: return 5;
                case MouseButtons.Middle | MouseButtons.Right: return 6;
                case MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right: return 7;
                default: return 0;
            }
        }

        private int mouseshift2int(MouseEventArgs arg)
        {
            int shift = (arg.Shift ? 1 : 0) +
            (arg.Control ? 2 : 0) +
            (arg.Alt ? 4 : 0);
            return shift;
        }

        private int keyshift2int(KeyEventArgs arg)
        {
            switch (arg.KeyCode)
            {
                case Keys.None: return 0;
                case Keys.Shift: return 1;
                case Keys.Control: return 2;
                case Keys.Shift | Keys.Control: return 3;
                case Keys.Alt: return 4;
                case Keys.Shift | Keys.Alt: return 5;
                case Keys.Control | Keys.Alt: return 6;
                case Keys.Shift | Keys.Control | Keys.Alt: return 7;
                default: return 0;
            }
        }
        #endregion

        #region ISketchTool Members
        void ISketchTool.AddPoint(IPoint point, bool Clone, bool allowUndo)
        {
            if (m_csc != null)
                m_csc.AddPoint(point, Clone, allowUndo);
        }

        IPoint ISketchTool.Anchor
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Anchor;
                else
                    return null;

            }
        }

        double ISketchTool.AngleConstraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.AngleConstraint;
                else
                    return 0.0;
            }
            set { m_csc.AngleConstraint = value; }
        }

        esriSketchConstraint ISketchTool.Constraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Constraint;
                else
                    return esriSketchConstraint.esriConstraintNone;

            }
            set { m_csc.Constraint = value; }
        }

        double ISketchTool.DistanceConstraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.DistanceConstraint;
                else
                    return 0.0;

            }
            set { m_csc.DistanceConstraint = value; }
        }

        bool ISketchTool.IsStreaming
        {
            get
            {
                if (m_csc != null)
                    return m_csc.IsStreaming;
                else
                    return false;

            }
            set { m_csc.IsStreaming = value; }
        }

        IPoint ISketchTool.Location
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Location;
                else
                    return null;

            }
        }
        #endregion
    }

    public partial class AddPointSplitLine
    {
        #region Tool input overriding methods
        protected sealed override void OnMouseDown(MouseEventArgs arg)
        {
            m_csc.OnMouseDown(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseMove(MouseEventArgs arg)
        {
            m_csc.OnMouseMove(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseUp(MouseEventArgs arg)
        {
            m_csc.OnMouseUp(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnKeyDown(KeyEventArgs arg)
        {
            m_csc.OnKeyDown((int)arg.KeyCode, keyshift2int(arg));
        }
        protected sealed override void OnKeyUp(KeyEventArgs arg)
        {
            m_csc.OnKeyUp((int)arg.KeyCode, keyshift2int(arg));
        }

        protected sealed override bool OnContextMenu(int x, int y)
        {
            return m_csc.OnContextMenu(x, y);
        }

        protected sealed override void OnRefresh(int hDC)
        {
            if (m_csc != null)
                m_csc.Refresh(hDC);
        }

        //int translations
        private int mousebutton2int(MouseEventArgs arg)
        {
            switch (arg.Button)
            {
                case MouseButtons.Left: return 1;
                case MouseButtons.Right: return 2;
                case MouseButtons.Left | MouseButtons.Right: return 3;
                case MouseButtons.Middle: return 4;
                case MouseButtons.Left | MouseButtons.Middle: return 5;
                case MouseButtons.Middle | MouseButtons.Right: return 6;
                case MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right: return 7;
                default: return 0;
            }
        }

        private int mouseshift2int(MouseEventArgs arg)
        {
            int shift = (arg.Shift ? 1 : 0) +
            (arg.Control ? 2 : 0) +
            (arg.Alt ? 4 : 0);
            return shift;
        }

        private int keyshift2int(KeyEventArgs arg)
        {
            switch (arg.KeyCode)
            {
                case Keys.None: return 0;
                case Keys.Shift: return 1;
                case Keys.Control: return 2;
                case Keys.Shift | Keys.Control: return 3;
                case Keys.Alt: return 4;
                case Keys.Shift | Keys.Alt: return 5;
                case Keys.Control | Keys.Alt: return 6;
                case Keys.Shift | Keys.Control | Keys.Alt: return 7;
                default: return 0;
            }
        }
        #endregion

        #region ISketchTool Members
        void ISketchTool.AddPoint(IPoint point, bool Clone, bool allowUndo)
        {
            if (m_csc != null)
                m_csc.AddPoint(point, Clone, allowUndo);
        }

        IPoint ISketchTool.Anchor
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Anchor;
                else
                    return null;

            }
        }

        double ISketchTool.AngleConstraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.AngleConstraint;
                else
                    return 0.0;
            }
            set { m_csc.AngleConstraint = value; }
        }

        esriSketchConstraint ISketchTool.Constraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Constraint;
                else
                    return esriSketchConstraint.esriConstraintNone;

            }
            set { m_csc.Constraint = value; }
        }

        double ISketchTool.DistanceConstraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.DistanceConstraint;
                else
                    return 0.0;

            }
            set { m_csc.DistanceConstraint = value; }
        }

        bool ISketchTool.IsStreaming
        {
            get
            {
                if (m_csc != null)
                    return m_csc.IsStreaming;
                else
                    return false;

            }
            set { m_csc.IsStreaming = value; }
        }

        IPoint ISketchTool.Location
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Location;
                else
                    return null;

            }
        }
        #endregion
    }

    public partial class ConnectClosestsConstructTool
    {
        #region Tool input overriding methods
        protected sealed override void OnMouseDown(MouseEventArgs arg)
        {
            m_csc.OnMouseDown(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseMove(MouseEventArgs arg)
        {
            m_csc.OnMouseMove(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseUp(MouseEventArgs arg)
        {
            m_csc.OnMouseUp(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnKeyDown(KeyEventArgs arg)
        {
            m_csc.OnKeyDown((int)arg.KeyCode, keyshift2int(arg));
        }
        protected sealed override void OnKeyUp(KeyEventArgs arg)
        {
            m_csc.OnKeyUp((int)arg.KeyCode, keyshift2int(arg));
        }

        protected sealed override bool OnContextMenu(int x, int y)
        {
            return m_csc.OnContextMenu(x, y);
        }

        protected sealed override void OnRefresh(int hDC)
        {
            if (m_csc != null)
                m_csc.Refresh(hDC);
        }

        //int translations
        private int mousebutton2int(MouseEventArgs arg)
        {
            switch (arg.Button)
            {
                case MouseButtons.Left: return 1;
                case MouseButtons.Right: return 2;
                case MouseButtons.Left | MouseButtons.Right: return 3;
                case MouseButtons.Middle: return 4;
                case MouseButtons.Left | MouseButtons.Middle: return 5;
                case MouseButtons.Middle | MouseButtons.Right: return 6;
                case MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right: return 7;
                default: return 0;
            }
        }

        private int mouseshift2int(MouseEventArgs arg)
        {
            int shift = (arg.Shift ? 1 : 0) +
            (arg.Control ? 2 : 0) +
            (arg.Alt ? 4 : 0);
            return shift;
        }

        private int keyshift2int(KeyEventArgs arg)
        {
            switch (arg.KeyCode)
            {
                case Keys.None: return 0;
                case Keys.Shift: return 1;
                case Keys.Control: return 2;
                case Keys.Shift | Keys.Control: return 3;
                case Keys.Alt: return 4;
                case Keys.Shift | Keys.Alt: return 5;
                case Keys.Control | Keys.Alt: return 6;
                case Keys.Shift | Keys.Control | Keys.Alt: return 7;
                default: return 0;
            }
        }
        #endregion

        #region ISketchTool Members
        void ISketchTool.AddPoint(IPoint point, bool Clone, bool allowUndo)
        {
            if (m_csc != null)
                m_csc.AddPoint(point, Clone, allowUndo);
        }

        IPoint ISketchTool.Anchor
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Anchor;
                else
                    return null;

            }
        }

        double ISketchTool.AngleConstraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.AngleConstraint;
                else
                    return 0.0;
            }
            set { m_csc.AngleConstraint = value; }
        }

        esriSketchConstraint ISketchTool.Constraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Constraint;
                else
                    return esriSketchConstraint.esriConstraintNone;

            }
            set { m_csc.Constraint = value; }
        }

        double ISketchTool.DistanceConstraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.DistanceConstraint;
                else
                    return 0.0;

            }
            set { m_csc.DistanceConstraint = value; }
        }

        bool ISketchTool.IsStreaming
        {
            get
            {
                if (m_csc != null)
                    return m_csc.IsStreaming;
                else
                    return false;

            }
            set { m_csc.IsStreaming = value; }
        }

        IPoint ISketchTool.Location
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Location;
                else
                    return null;

            }
        }
        #endregion
    }
    public partial class PointsAlongLineTool
    {
        #region Tool input overriding methods
        protected sealed override void OnMouseDown(MouseEventArgs arg)
        {
            m_csc.OnMouseDown(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseMove(MouseEventArgs arg)
        {
            m_csc.OnMouseMove(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnMouseUp(MouseEventArgs arg)
        {
            m_csc.OnMouseUp(mousebutton2int(arg), mouseshift2int(arg), arg.X, arg.Y);
        }

        protected sealed override void OnKeyDown(KeyEventArgs arg)
        {
            m_csc.OnKeyDown((int)arg.KeyCode, keyshift2int(arg));
        }
        protected sealed override void OnKeyUp(KeyEventArgs arg)
        {
            m_csc.OnKeyUp((int)arg.KeyCode, keyshift2int(arg));
        }

        protected sealed override bool OnContextMenu(int x, int y)
        {
            return m_csc.OnContextMenu(x, y);
        }

        protected sealed override void OnRefresh(int hDC)
        {
            if (m_csc != null)
                m_csc.Refresh(hDC);
        }

        //int translations
        private int mousebutton2int(MouseEventArgs arg)
        {
            switch (arg.Button)
            {
                case MouseButtons.Left: return 1;
                case MouseButtons.Right: return 2;
                case MouseButtons.Left | MouseButtons.Right: return 3;
                case MouseButtons.Middle: return 4;
                case MouseButtons.Left | MouseButtons.Middle: return 5;
                case MouseButtons.Middle | MouseButtons.Right: return 6;
                case MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right: return 7;
                default: return 0;
            }
        }

        private int mouseshift2int(MouseEventArgs arg)
        {
            int shift = (arg.Shift ? 1 : 0) +
            (arg.Control ? 2 : 0) +
            (arg.Alt ? 4 : 0);
            return shift;
        }

        private int keyshift2int(KeyEventArgs arg)
        {
            switch (arg.KeyCode)
            {
                case Keys.None: return 0;
                case Keys.Shift: return 1;
                case Keys.Control: return 2;
                case Keys.Shift | Keys.Control: return 3;
                case Keys.Alt: return 4;
                case Keys.Shift | Keys.Alt: return 5;
                case Keys.Control | Keys.Alt: return 6;
                case Keys.Shift | Keys.Control | Keys.Alt: return 7;
                default: return 0;
            }
        }
        #endregion

        #region ISketchTool Members
        void ISketchTool.AddPoint(IPoint point, bool Clone, bool allowUndo)
        {
            if (m_csc != null)
                m_csc.AddPoint(point, Clone, allowUndo);
        }

        IPoint ISketchTool.Anchor
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Anchor;
                else
                    return null;

            }
        }

        double ISketchTool.AngleConstraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.AngleConstraint;
                else
                    return 0.0;
            }
            set { m_csc.AngleConstraint = value; }
        }

        esriSketchConstraint ISketchTool.Constraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Constraint;
                else
                    return esriSketchConstraint.esriConstraintNone;

            }
            set { m_csc.Constraint = value; }
        }

        double ISketchTool.DistanceConstraint
        {
            get
            {
                if (m_csc != null)
                    return m_csc.DistanceConstraint;
                else
                    return 0.0;

            }
            set { m_csc.DistanceConstraint = value; }
        }

        bool ISketchTool.IsStreaming
        {
            get
            {
                if (m_csc != null)
                    return m_csc.IsStreaming;
                else
                    return false;

            }
            set { m_csc.IsStreaming = value; }
        }

        IPoint ISketchTool.Location
        {
            get
            {
                if (m_csc != null)
                    return m_csc.Location;
                else
                    return null;

            }
        }
        #endregion
    }

}
