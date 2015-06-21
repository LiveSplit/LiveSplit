using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class UserNameStyle
    {
        public bool IsGradient { get; private set; }
        public string LightSolidColorCode { get; private set; }
        public string LightGradientStartColorCode
        {
            get { return LightSolidColorCode; }
            private set { LightSolidColorCode = value; }
        }
        public string LightGradientEndColorCode { get; private set; }
        public string DarkSolidColorCode { get; private set; }
        public string DarkGradientStartColorCode
        {
            get { return DarkSolidColorCode; }
            private set { DarkSolidColorCode = value; }
        }
        public string DarkGradientEndColorCode { get; private set; }

        private UserNameStyle() { }

        public static UserNameStyle Parse(SpeedrunComClient client, dynamic styleElement)
        {
            var style = new UserNameStyle();

            style.IsGradient = styleElement.style == "gradient";

            if (style.IsGradient)
            {
                var properties = styleElement.Properties as IDictionary<string, dynamic>;
                var colorFrom = properties["color-from"];
                var colorTo = properties["color-to"];

                style.LightGradientStartColorCode = colorFrom.light as string;
                style.LightGradientEndColorCode = colorTo.light as string;
                style.DarkGradientStartColorCode = colorFrom.dark as string;
                style.DarkGradientEndColorCode = colorTo.dark as string;
            }
            else
            {
                style.LightSolidColorCode = styleElement.color.light as string;
                style.DarkSolidColorCode = styleElement.color.dark as string;
            }

            return style;
        }
    }
}
