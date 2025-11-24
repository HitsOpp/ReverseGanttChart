import { useState } from "react";
import { SidePanel } from "./components";
import { AppRouter } from "./routes";
import { useQuery } from "@tanstack/react-query";
import { loadProfileData } from "./api";
import { BrowserRouter } from "react-router";

export const App = () => {
  const [panelOpen, setPanelOpen] = useState(true);
  const { data } = useQuery(loadProfileData());
  return (
    <BrowserRouter>
      <SidePanel
        userName={data?.fullName || "..."}
        isOpen={panelOpen}
        setIsOpen={setPanelOpen}
      />
      <main
        className={`
          transition-all duration-300
          ${panelOpen ? "lg:ml-80" : "lg:ml-0"}
        `}
      >
        <AppRouter />;
      </main>
    </BrowserRouter>
  );
};
