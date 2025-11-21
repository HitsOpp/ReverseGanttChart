import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { AuthApi } from "client/utils";
import { FormBlock, type FormField } from "./FormBlock";
import type { AxiosError } from "axios";

interface RegisterFormType {
  fullName: string;
  email: string;
  password: string;
}

const schema = yup.object({
  fullName: yup.string().min(3, "Минимум 3 символа").required("Введите ФИО"),
  email: yup.string().email("Некорректный email").required("Введите email"),
  password: yup
    .string()
    .min(8, "Минимум 8 символов")
    .required("Введите пароль"),
});

const fields: FormField<RegisterFormType>[] = [
  { name: "fullName", placeholder: "Введите ФИО", label: "ФИО" },
  { name: "email", placeholder: "Введите email", label: "Email" },
  { name: "password", placeholder: "Введите пароль", label: "Пароль" },
];

export const RegisterForm = () => {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm<RegisterFormType>({
    resolver: yupResolver(schema),
  });

  const onSubmit = handleSubmit(async (formData) => {
    try {
      const { accessToken, user } = await AuthApi.register(formData);

      localStorage.setItem("accessToken", accessToken);

      reset();
      alert("Аккаунт создан! Добро пожаловать, " + user.fullName);
    } catch (error) {
      const err = error as AxiosError<{ message?: string }>;

      const message =
        err.response?.data?.message || err.message || "Ошибка регистрации";

      alert(message);
      console.error(err);
    }
  });

  return (
    <FormBlock<RegisterFormType>
      title="Регистрация"
      fields={fields}
      submitText="Зарегистрироваться"
      onSubmit={onSubmit}
      register={register}
      errors={errors}
      isSubmitting={isSubmitting}
    />
  );
};
