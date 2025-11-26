import { useState } from "react";

export const useLocalStorage = <T>(
  key: string,
  initialValue: T
): [T, (value: T) => void] => {
  const [storedValue, setStoredValue] = useState<T>(() => {
    if (typeof window === "undefined") return initialValue;
    try {
      const item = window.localStorage.getItem(key);
      return item ? (JSON.parse(item) as T) : initialValue;
    } catch {
      return initialValue;
    }
  });

  const setValue = (value: T) => {
    setStoredValue(value);
    try {
      window.localStorage.setItem(key, JSON.stringify(value));
    } catch {
      console.error("Failed to set localStorage item");
    }
  };

  return [storedValue, setValue];
};
