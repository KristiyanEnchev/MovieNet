import { useState, useCallback } from 'react';

function useFormValidation(schema, fields) {
  const [errors, setErrors] = useState({});

  const validate = useCallback(
    (data) => {
      if (!schema) return true;

      try {
        schema.parse(data);
        setErrors({});
        return true;
      } catch (error) {
        const formattedErrors = {};
        error.errors.forEach((err) => {
          formattedErrors[err.path[0]] = err.message;
        });
        setErrors(formattedErrors);
        return false;
      }
    },
    [schema]
  );

  const clearError = useCallback((field) => {
    setErrors((prev) => {
      const newErrors = { ...prev };
      delete newErrors[field];
      return newErrors;
    });
  }, []);

  const getFieldError = useCallback(
    (field) => errors[field],
    [errors]
  );

  const isValid = useCallback(
    () => Object.keys(errors).length === 0,
    [errors]
  );

  return {
    validate,
    clearError,
    getFieldError,
    isValid,
    errors,
  };
}

export default useFormValidation;
