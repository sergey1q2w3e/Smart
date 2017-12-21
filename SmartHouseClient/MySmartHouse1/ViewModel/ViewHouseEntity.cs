using System;
using System.Collections.Generic;
using System.Linq;
using MySmartHouse1.DataModel;
using MySmartHouse1.Model;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MySmartHouse1
{
    public class ViewHouseEntity : Notifier
    {
        public HouseEntity HouseEntity { get; set; }
        public PlotModel StatPlotModel { get; private set; }

        private List<StatisticsHT> _statistics;
        private int _isParameterBusy;
        private bool _isRefreshBusy;

        public List<StatisticsHT> Statistics
        {
            get { return _statistics; }
            set
            {
                if (!_statistics.Equals(value))
                {
                    _statistics = value;
                    FillPlotData();
                    OnPropertyChanged("Statistics");
                }
            }
        }
        public int IsParameterBusy
        {
            get { return _isParameterBusy; }
            set
            {
                if (_isParameterBusy != value)
                {
                    _isParameterBusy = value;
                    OnPropertyChanged("IsParameterBusy");
                }
            }
        }

        public bool IsRefreshBusy
        {
            get { return _isRefreshBusy; }
            set
            {
                if (_isRefreshBusy != value)
                {
                    _isRefreshBusy = value;
                    OnPropertyChanged("IsRefreshBusy");
                }
            }
        }
        public ViewHouseEntity()
        {
            HouseEntity = new HouseEntity();
            _statistics = new List<StatisticsHT>();
            StatPlotModel = new PlotModel
            {
                Title = "Статистика",
                Axes = {new DateTimeAxis {Position = AxisPosition.Bottom, StringFormat = "dd.MM.yyyy HH:mm"}}
                
            };
            //StatPlotModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
        }

        private void FillPlotData()
        {
            if (_statistics != null && _statistics.Any())
            {
                StatPlotModel.Axes[0].Minimum = DateTimeAxis.ToDouble(_statistics[0].ValueDateTime);
                StatPlotModel.Axes[0].Maximum = DateTimeAxis.ToDouble(_statistics[_statistics.Count - 1].ValueDateTime);
                LineSeries lineH = new LineSeries()
                {
                    Color = OxyColors.Blue,
                    LineJoin = LineJoin.Round,
                    Title = "Влажность"
                };
                LineSeries lineT = new LineSeries()
                {
                    Color = OxyColors.Red,
                    LineJoin = LineJoin.Round,
                    Title = "Температура"
                };

                foreach (var stat in _statistics)
                {
                    lineH.Points.Add(new DataPoint(DateTimeAxis.ToDouble(stat.ValueDateTime), stat.Humidity));
                    lineT.Points.Add(new DataPoint(DateTimeAxis.ToDouble(stat.ValueDateTime), stat.Temperature));
                }
                if (StatPlotModel.Series.Any())
                    StatPlotModel.Series.Clear();
                StatPlotModel.Series.Add(lineT);
                StatPlotModel.Series.Add(lineH);
                StatPlotModel.InvalidatePlot(true);
            }
        }
    }
}
