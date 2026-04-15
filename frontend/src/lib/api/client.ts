import axios, { AxiosError, type AxiosInstance } from "axios";

/**
 * Standard error body returned by the EAP backend (see Eap.Api.Conventions.ApiError).
 */
export interface ApiError {
  status: number;
  title: string;
  detail?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
}

const baseURL =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5100";

export const apiClient: AxiosInstance = axios.create({
  baseURL,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
  timeout: 30_000,
});

apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ApiError>) => {
    // Normalize errors so consumers can rely on a predictable shape.
    const normalized: ApiError = error.response?.data ?? {
      status: error.response?.status ?? 0,
      title: error.message || "Network error",
    };
    return Promise.reject(normalized);
  },
);
