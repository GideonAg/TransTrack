namespace Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ErrorMessages { get; set; }
        public string FileName { get; set; }

        public ValidationResult()
        {
            ErrorMessages = new List<string>();
            FileName = string.Empty;
        }

        public ValidationResult(bool isValid)
        {
            IsValid = isValid;
            ErrorMessages = new List<string>();
            FileName = string.Empty;
        }

        public void AddError(string errorMessage)
        {
            IsValid = false;
            ErrorMessages.Add(errorMessage);
        }

        public string GetErrorSummary()
        {
            return string.Join("; ", ErrorMessages);
        }
    }
}
