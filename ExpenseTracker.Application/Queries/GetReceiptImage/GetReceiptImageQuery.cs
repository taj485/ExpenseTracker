using ExpenseTracker.Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Queries.GetReceiptImage
{
    public class GetReceiptImageQuery : IRequest<ReceiptImageDto>
    {
        public int ReceiptId { get; set; }
        public int ExpenseTableId { get; set; }
    }
}
