import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import axios from "axios";
import { FormBlock, type FormField } from "./FormBlock";

interface RegisterForm {
  fullName: string;
  email: string;
  password: string;
}

const schema = yup.object({
  fullName: yup.string().required("Введите ФИО").min(3, "Минимум 3 символа"),
  email: yup.string().email("Некорректный email").required("Введите email"),
  password: yup.string().length(8).required("Введите пароль"),
});

const fields: FormField<RegisterForm>[] = [
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
  } = useForm<RegisterForm>({
    resolver: yupResolver(schema),
  });

  const onSubmit = handleSubmit(async (data) => {
    await axios.post("/api/feedback", data);
    reset();
    alert("Успешно отправлено!");
  });

  return (
    <FormBlock<RegisterForm>
      title="Регистрация"
      fields={fields}
      submitText="Отправить"
      onSubmit={onSubmit}
      register={register}
      errors={errors}
      isSubmitting={isSubmitting}
    />
  );
};
