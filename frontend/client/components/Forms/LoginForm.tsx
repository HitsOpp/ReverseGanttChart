import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { AuthApi } from "client/utils";
import { FormBlock, type FormField } from "./FormBlock";
import type { AxiosError } from "axios";
import { useNavigate } from "react-router";

interface LoginFormType {
  email: string;
  password: string;
}

const schema = yup.object({
  email: yup.string().email("Некорректный email").required("Введите email"),
  password: yup
    .string()
    .min(8, "Минимум 8 символов")
    .required("Введите пароль"),
});

const fields: FormField<LoginFormType>[] = [
  { name: "email", placeholder: "Введите email", label: "Email" },
  { name: "password", placeholder: "Введите пароль", label: "Пароль" },
];

export const LoginForm = () => {
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm<LoginFormType>({
    resolver: yupResolver(schema),
  });

  const onSubmit = handleSubmit(async (formData) => {
    try {
      const response = await AuthApi.login(formData);
      console.log(response);
      const { token } = response;
      localStorage.setItem("accessToken", token);

      reset();
      navigate("/subjects");
    } catch (error) {
      const err = error as AxiosError<{ message?: string }>;

      const message =
        err.response?.data?.message || err.message || "Ошибка входа";

      alert(message);
      console.error(err);
    }
  });

  return (
    <FormBlock<LoginFormType>
      title="Добро пожаловать"
      fields={fields}
      submitText="Войти"
      onSubmit={onSubmit}
      register={register}
      errors={errors}
      isSubmitting={isSubmitting}
    />
  );
};
