using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using WindowsRegistryUpdater.Operations.Operate;

namespace WindowsRegistryUpdater.WPFConverters
{
    public class OperateStateToBrushConverter : IValueConverter
    {
        private Brush NothingBrush = new SolidColorBrush(Colors.White);
        private Brush SuccessBrush = new SolidColorBrush(Colors.AliceBlue);
        private Brush FailedBrush = new SolidColorBrush(Colors.Red);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is OperateState?))
                throw new NotImplementedException("Only OperateState");
            var Value = (OperateState)(value as OperateState?);
            switch (Value)
            {
                case OperateState.NothingWas:
                    return NothingBrush;
                case OperateState.Success:
                    return SuccessBrush;
                case OperateState.Failed:
                    return FailedBrush;
                default:
                    throw new NotImplementedException("OperateState unknown");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
