import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5261",
  withCredentials: true,
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

let isRefreshing = false;
let queue: ((token: string) => void)[] = [];

api.interceptors.response.use(
  (res) => res,
  async (error) => {
    const original = error.config;

    if (error.response?.status === 401 && !original._retry) {
      original._retry = true;

      if (!isRefreshing) {
        isRefreshing = true;

        try {
          const { accessToken } = await axios
            .post(
              `${import.meta.env.VITE_API_URL}/Auth/refresh`,
              {},
              { withCredentials: true }
            )
            .then((r) => r.data);

          localStorage.setItem("accessToken", accessToken);

          queue.forEach((cb) => cb(accessToken));
          queue = [];

          isRefreshing = false;

          original.headers = original.headers || {};
          original.headers.Authorization = `Bearer ${accessToken}`;

          return api(original);
        } catch (err) {
          queue = [];
          isRefreshing = false;
          localStorage.removeItem("accessToken");
          return Promise.reject(err);
        }
      }

      return new Promise((resolve) => {
        queue.push((token: string) => {
          original.headers = original.headers || {};
          original.headers.Authorization = `Bearer ${token}`;
          resolve(api(original));
        });
      });
    }

    return Promise.reject(error);
  }
);

export const apiCall = {
  get: async <T>(url: string, config?: object): Promise<T> => {
    const res = await api.get<T>(url, config);
    return res.data;
  },

  post: async <T>(url: string, data?: object, config?: object): Promise<T> => {
    const res = await api.post<T>(url, data, config);
    return res.data;
  },

  put: async <T>(url: string, data?: object, config?: object): Promise<T> => {
    const res = await api.put<T>(url, data, config);
    return res.data;
  },

  delete: async <T>(url: string, config?: object): Promise<T> => {
    const res = await api.delete<T>(url, config);
    return res.data;
  },
};
