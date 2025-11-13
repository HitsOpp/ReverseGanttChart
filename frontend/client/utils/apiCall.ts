import axios, { type AxiosRequestConfig } from "axios";

const axiosInstance = axios.create({
  baseURL: "localhost",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

export const apiCall = {
  async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const { data } = await axiosInstance.get<T>(url, config);
    return data;
  },

  async post<T, B = unknown>(
    url: string,
    body?: B,
    config?: AxiosRequestConfig
  ): Promise<T> {
    const { data } = await axiosInstance.post<T>(url, body, config);
    return data;
  },

  async put<T, B = unknown>(
    url: string,
    body?: B,
    config?: AxiosRequestConfig
  ): Promise<T> {
    const { data } = await axiosInstance.put<T>(url, body, config);
    return data;
  },

  async patch<T, B = unknown>(
    url: string,
    body?: B,
    config?: AxiosRequestConfig
  ): Promise<T> {
    const { data } = await axiosInstance.patch<T>(url, body, config);
    return data;
  },

  async delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const { data } = await axiosInstance.delete<T>(url, config);
    return data;
  },
};
