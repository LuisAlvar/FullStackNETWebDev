using ExcelNextLoanPaymentInfo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelNextLoanPaymentInfo;

public class LoanManager
{
  public List<Loan> loans { get; set; } = new List<Loan>();
}
