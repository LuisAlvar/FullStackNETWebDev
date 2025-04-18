using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelNextLoanPaymentInfo.Entities;

public class Loan
{
  public List<LoanProp> Props { get; set; } = new List<LoanProp>();
  public string Name { get; set; } = string.Empty;


}
