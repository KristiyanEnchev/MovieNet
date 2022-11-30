import { memo, useCallback } from 'react';
import PropTypes from 'prop-types';
import FormField from './FormField';
import useFormFields from '../../hooks/useFormFields';
import useFormValidation from '../../hooks/useFormValidation';
import { cn } from '../../lib/utils';

const Form = memo(
  ({
    initialValues,
    validationSchema,
    onSubmit,
    submitButtonText,
    fields,
    titleSection,
    extraContent,
  }) => {
    const {
      fields: formFields,
      createChangeHandler,
      resetFields,
      capitalizeFirstLetter,
    } = useFormFields(initialValues);

    const { validate, clearError, getFieldError } = useFormValidation(
      validationSchema,
      formFields
    );

    const handleSubmit = useCallback(
      async (e) => {
        e.preventDefault();
        if (validate(formFields)) {
          await onSubmit(formFields);
          resetFields();
        }
      },
      [validate, formFields, onSubmit, resetFields]
    );

    return (
      <div className="w-full max-w-md mx-auto">
        <div className="bg-card border border-border rounded-lg shadow-sm p-6 space-y-4">
          {titleSection}
          <form onSubmit={handleSubmit} className="space-y-4">
            {fields.map(({ name, type, label }) => (
              <FormField
                key={name}
                label={label || capitalizeFirstLetter(name)}
                type={type || name}
                value={formFields[name]}
                onChange={createChangeHandler(name, () => clearError(name))}
                onFocus={() => clearError(name)}
                error={getFieldError(name)}
                id={name}
              />
            ))}
            <button
              type="submit"
              className={cn(
                'w-full py-2.5 px-4 bg-primary text-primary-foreground',
                'rounded-md font-medium shadow-sm',
                'hover:bg-primary/90 focus:outline-none focus:ring-2',
                'focus:ring-primary/30 transition-colors'
              )}
            >
              {submitButtonText}
            </button>
          </form>
          {extraContent && (
            <div className="mt-4 text-center text-sm text-muted-foreground">
              {extraContent}
            </div>
          )}
        </div>
      </div>
    );
  }
);

Form.displayName = 'Form';

Form.propTypes = {
  initialValues: PropTypes.object.isRequired,
  validationSchema: PropTypes.object,
  onSubmit: PropTypes.func.isRequired,
  submitButtonText: PropTypes.string.isRequired,
  fields: PropTypes.array.isRequired,
  titleSection: PropTypes.node,
  extraContent: PropTypes.node,
};

export default Form;
