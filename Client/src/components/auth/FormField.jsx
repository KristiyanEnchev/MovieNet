import { useState, memo } from 'react';
import PropTypes from 'prop-types';
import PasswordStrengthBar from 'react-password-strength-bar';

const FormField = memo(
  ({ label, type, value, onChange, onFocus, error, id, ...props }) => {
    const [showPassword, setShowPassword] = useState(false);
    const isPasswordField = type === 'password' || type === 'confirmPassword';
    const inputType = isPasswordField
      ? showPassword
        ? 'text'
        : 'password'
      : type;
    const isPassword = type === 'password';

    const togglePasswordVisibility = () => setShowPassword((prev) => !prev);

    return (
      <div className="mb-4">
        <div className="relative">
          <label
            htmlFor={id}
            className="text-foreground/70 block mb-1.5 text-sm"
          >
            {label}
          </label>
          <input
            id={id}
            type={inputType}
            value={value}
            onChange={onChange}
            onFocus={onFocus}
            className="w-full px-3 py-2 bg-input border border-input rounded-md focus:outline-none focus:ring-2 focus:ring-primary/30 text-foreground"
            {...props}
          />
          {isPasswordField && (
            <button
              onClick={togglePasswordVisibility}
              type="button"
              className="absolute right-3 top-[34px] text-xs text-foreground/70 hover:text-foreground"
            >
              {showPassword ? 'Hide' : 'Show'}
            </button>
          )}
          {isPassword && <PasswordStrengthBar password={value} />}
          {error && (
            <span className="text-destructive text-sm mt-1">{error}</span>
          )}
        </div>
      </div>
    );
  }
);

FormField.displayName = 'FormField';

FormField.propTypes = {
  label: PropTypes.string.isRequired,
  type: PropTypes.string.isRequired,
  value: PropTypes.string.isRequired,
  onChange: PropTypes.func.isRequired,
  onFocus: PropTypes.func,
  error: PropTypes.string,
  id: PropTypes.string.isRequired,
};

export default FormField;
