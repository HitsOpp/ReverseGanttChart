import { useLocalStorage } from "./hooks";
import { SidePanel } from "./components";
import { AppRouter } from "./routes";
import { BrowserRouter, useLocation } from "react-router-dom";

const AppContent = () => {
  const [panelOpen, setPanelOpen] = useLocalStorage<boolean>("panelOpen", true);
  const location = useLocation();

  return (
    <>
      {location.pathname !== "/login" && (
        <SidePanel isOpen={panelOpen} setIsOpen={setPanelOpen} />
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
