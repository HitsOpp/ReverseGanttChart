using System.ComponentModel.DataAnnotations;

public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateGreaterThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        ErrorMessage = ErrorMessageString;
        var currentValue = (DateTime)value;

        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (property == null)
            throw new ArgumentException($"Property {_comparisonProperty} not found");

        var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

        if (currentValue <= comparisonValue)
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}