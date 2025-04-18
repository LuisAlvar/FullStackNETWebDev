using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelNextLoanPaymentInfo.Entities;

/// <summary>
/// Use to ensucaplate a loan property such as Loan Type or Due Date
/// </summary>
public class LoanProp
{
  public string DataType { get; set; } = string.Empty;

  public string DataValue { get; set; } = string.Empty;
}
