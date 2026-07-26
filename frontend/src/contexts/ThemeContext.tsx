import { createContext, useCallback, useEffect, useMemo, useState, type PropsWithChildren } from "react";

export type Theme = "light" | "dark";
export type ThemePreference = Theme | "system";
interface ThemeContextValue { theme:Theme; preference:ThemePreference; toggleTheme:()=>void; setThemePreference:(value:ThemePreference)=>void; }
export const ThemeContext=createContext<ThemeContextValue|null>(null);
const initial=():ThemePreference=>{const saved=localStorage.getItem("coding-theme");return saved==="light"||saved==="dark"||saved==="system"?saved:"system"};
const systemTheme=():Theme=>window.matchMedia("(prefers-color-scheme: dark)").matches?"dark":"light";

export function ThemeProvider({children}:PropsWithChildren){
  const[preference,setPreference]=useState<ThemePreference>(initial);
  const[system,setSystem]=useState<Theme>(systemTheme);
  const theme:Theme=preference==="system"?system:preference;
  useEffect(()=>{
    const media=window.matchMedia("(prefers-color-scheme: dark)");
    const change=()=>setSystem(media.matches?"dark":"light");
    change();
    media.addEventListener("change",change);
    window.addEventListener("focus",change);
    document.addEventListener("visibilitychange",change);
    return()=>{
      media.removeEventListener("change",change);
      window.removeEventListener("focus",change);
      document.removeEventListener("visibilitychange",change);
    };
  },[]);
  useEffect(()=>{
    document.documentElement.dataset.theme=theme;
    document.documentElement.dataset.themePreference=preference;
    document.documentElement.style.colorScheme=theme;
    localStorage.setItem("coding-theme",preference);
  },[preference,theme]);
  const setThemePreference=useCallback((value:ThemePreference)=>setPreference(value),[]);
  const toggleTheme=useCallback(()=>setPreference(current=>(current==="system"?system:current)==="light"?"dark":"light"),[system]);
  const value=useMemo(()=>({theme,preference,toggleTheme,setThemePreference}),[theme,preference,toggleTheme,setThemePreference]);
  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
}
