using FedexRatesAndTime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FedexRatesAndTime.Pages {
    public class RatesAndTransitTimesPage : AbstractPage {
        protected const string PageURL = "https://www.fedex.com/ratefinder/home?cc=US&language=en&locId=express";
        public RatesAndTransitTimesPage() : base(PageURL) {

        }
    }
}
