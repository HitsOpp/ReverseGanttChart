import { useLocalStorage } from "./hooks";
import { SidePanel } from "./components";
import { AppRouter } from "./routes";
import { useQuery } from "@tanstack/react-query";
import { loadProfileData } from "./api";
import { BrowserRouter, useLocation } from "react-router-dom";

const AppContent = () => {
  const [panelOpen, setPanelOpen] = useLocalStorage<boolean>("panelOpen", true);
  const { data } = useQuery(loadProfileData());
  const location = useLocation();

  return (
    <>
      {location.pathname !== "/login" && (
        <SidePanel
          userName={data?.fullName || "..."}
          isOpen={panelOpen}
          setIsOpen={setPanelOpen}
        />
      )}
      <main
        className={`
          transition-all duration-300
          ${
            panelOpen && location.pathname !== "/login" ? "lg:ml-80" : "lg:ml-0"
          }
        `}
      >
        <AppRouter />
      </main>
    </>
  );
};

export const App = () => (
  <BrowserRouter>
    <AppContent />
  </BrowserRouter>
);
