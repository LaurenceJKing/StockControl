using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockControl.Orders.Data;
using StockControl.Orders.Services;

namespace StockControl.Orders.Application
{

        

        public class DraftOrderViewModel
        {
            public int Id { get; set; }
            public IEnumerable<DraftOrderItemViewModel> Items { get; set; }
        }
    }
