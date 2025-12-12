import { useQuery } from "@tanstack/react-query";
import { AuthApi } from "client/utils";

export const useAuth = () => {
  return useQuery({
    queryKey: ["auth"],
    queryFn: AuthApi.me,
    enabled: !!localStorage.getItem("accessToken"),
    retry: false,
  });
};
