using System;
using System.ComponentModel.DataAnnotations;

namespace WaterBill.Models;

public class WaterConsumption
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [Display(Name = "Meter Reading")]
    public int MeterReading { get; set; }

    [Required]
    [Display(Name = "Actual or Estimate")]
    public string ActualOrEstimate { get; set; }

    [Required]
    public double Charge { get; set; }
}
