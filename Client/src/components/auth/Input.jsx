import { useState } from 'react';
import { Eye, EyeOff } from 'lucide-react';
import PropTypes from 'prop-types';

const Input = ({
  icon: Icon,
  type,
  placeholder,
  value,
  onChange,
  error,
  ...props
}) => {
  const [showPassword, setShowPassword] = useState(false);
  const isPassword = type === 'password';

  return (
    <div className="mb-4">
      <div className="relative">
        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
          {Icon && <Icon className="h-5 w-5 text-gray-400" />}
        </div>
        <input
          type={isPassword ? (showPassword ? 'text' : 'password') : type}
          className={`w-full pl-10 pr-12 py-2.5 bg-gray-700 bg-opacity-50 backdrop-blur-sm text-white
            placeholder-gray-400 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/30
            ${error ? 'border-red-500 focus:ring-red-500' : 'border-gray-600'}`}
          placeholder={placeholder}
          value={value}
          onChange={onChange}
          {...props}
        />
        {isPassword && (
          <button
            type="button"
            onClick={() => setShowPassword(!showPassword)}
            className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-300"
          >
            {showPassword ? (
              <EyeOff className="h-5 w-5" />
            ) : (
              <Eye className="h-5 w-5" />
            )}
          </button>
        )}
      </div>
      {error && <p className="mt-1 text-sm text-red-500">{error}</p>}
    </div>
  );
};

Input.propTypes = {
  icon: PropTypes.elementType,
  type: PropTypes.string.isRequired,
  placeholder: PropTypes.string,
  value: PropTypes.string.isRequired,
  onChange: PropTypes.func.isRequired,
  error: PropTypes.string,
};

export default Input;
