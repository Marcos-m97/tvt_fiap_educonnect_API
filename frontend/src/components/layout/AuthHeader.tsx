import { Stack, Typography, IconButton } from "@mui/material";
import SchoolRoundedIcon from "@mui/icons-material/SchoolRounded";
import DarkModeRoundedIcon from "@mui/icons-material/DarkModeRounded";
import LightModeRoundedIcon from "@mui/icons-material/LightModeRounded";
import { useThemeApp } from "../../contexts/ThemeContext";

type AuthHeaderProps = {
  subtitle: string;
  showThemeToggle?: boolean;
  bottomThemeToggle?: boolean;
};

export default function AuthHeader({
  subtitle,
  showThemeToggle = true,
  bottomThemeToggle = false,
}: AuthHeaderProps) {
  const { mode, toggle } = useThemeApp();

  return (
    <Stack alignItems="center" spacing={1} mb={3} position="relative">
      {/* 🌙 Botão no topo (padrão) */}
      {showThemeToggle && !bottomThemeToggle && (
        <IconButton
          onClick={toggle}
          sx={{ position: "absolute", right: 0, top: 0 }}
          size="small"
        >
          {mode === "dark" ? (
            <LightModeRoundedIcon />
          ) : (
            <DarkModeRoundedIcon />
          )}
        </IconButton>
      )}

      <Stack direction="row" spacing={1} alignItems="center">
        <SchoolRoundedIcon sx={{ fontSize: 38, color: "primary.main" }} />
        <Typography variant="h4" fontWeight={700}>
          EduConnect
        </Typography>
      </Stack>

      <Typography variant="body2" color="text.secondary" textAlign="center">
        {subtitle}
      </Typography>

      {/* 🌙 Botão embaixo (login) */}
      {showThemeToggle && bottomThemeToggle && (
        <IconButton onClick={toggle} size="small" sx={{ mt: 1, opacity: 0.75 }}>
          {mode === "dark" ? (
            <LightModeRoundedIcon />
          ) : (
            <DarkModeRoundedIcon />
          )}
        </IconButton>
      )}
    </Stack>
  );
}
