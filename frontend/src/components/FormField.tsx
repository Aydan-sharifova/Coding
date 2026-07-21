import { forwardRef, type InputHTMLAttributes } from "react";

interface FormFieldProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
}

export const FormField = forwardRef<HTMLInputElement, FormFieldProps>(
  ({ label, error, id, ...inputProps }, ref) => {
    const inputId = id ?? inputProps.name;
    const errorId = error ? `${inputId}-error` : undefined;

    return (
      <div className="form-field">
        <label htmlFor={inputId}>{label}</label>
        <input
          {...inputProps}
          ref={ref}
          id={inputId}
          aria-invalid={Boolean(error)}
          aria-describedby={errorId}
        />
        {error && <span className="field-error" id={errorId} role="alert">{error}</span>}
      </div>
    );
  },
);

FormField.displayName = "FormField";
