import React from "react";
import {
  type FieldErrors,
  type UseFormRegister,
  type Path,
} from "react-hook-form";

export interface FormField<TFormValues> {
  name: Path<TFormValues>;
  placeholder: string;
  label?: string;
}

interface FormBlockProps<TFormValues extends Record<string, any>> {
  title: string;
  fields: FormField<TFormValues>[];
  submitText: string;
  onSubmit: () => void;
  register: UseFormRegister<TFormValues>;
  errors: FieldErrors<TFormValues>;
  isSubmitting?: boolean;
}

export const FormBlock = <TFormValues extends Record<string, any>>({
  title,
  fields,
  submitText,
  onSubmit,
  register,
  errors,
  isSubmitting = false,
}: FormBlockProps<TFormValues>) => {
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit();
  };

  return (
    <div className="max-w-md mx-auto mt-10 p-6 bg-white rounded-xl shadow-md border border-gray-200">
      <h2 className="text-2xl font-semibold mb-6 text-center text-gray-800">
        {title}
      </h2>

      <form onSubmit={handleSubmit} className="space-y-5">
        {fields.map((field) => (
          <div key={field.name} className="relative">
            {field.label && (
              <label className="block text-sm font-medium text-gray-700 mb-1">
                {field.label}
              </label>
            )}
            <input
              type="text"
              placeholder={field.placeholder}
              {...register(field.name)}
              className="w-full px-4 py-3 border border-gray-300 rounded-lg 
                         focus:outline-none focus:ring-2 focus:ring-indigo-400 
                         focus:border-indigo-400 transition duration-200 bg-gray-50
                         placeholder-gray-400 text-gray-800"
            />
            {errors[field.name] && (
              <p className="text-red-500 text-xs mt-1">
                {String(errors[field.name]?.message)}
              </p>
            )}
          </div>
        ))}

        <button
          type="submit"
          disabled={isSubmitting}
          className="w-full py-3 px-4 bg-indigo-600 text-white font-semibold rounded-lg shadow
                     hover:bg-indigo-700 transition-colors duration-200 focus:outline-none 
                     focus:ring-2 focus:ring-indigo-300 disabled:opacity-50"
        >
          {isSubmitting ? "Отправка..." : submitText}
        </button>
      </form>

      <div className="mt-4 text-center">
        <span className="text-xs text-gray-500">
          ⚡ Эта форма готова к использованию!
        </span>
      </div>
    </div>
  );
};
