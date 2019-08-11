using System;

namespace PigeonCoopToolkit.Generic.Editor.GUIHelper
{
    public class GUIWidget {

        protected readonly Action Repaint;

        public GUIWidget(Action repaintFn)
        {
            Repaint = repaintFn;
        }
	
    }
}
