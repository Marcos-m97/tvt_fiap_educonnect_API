import { Stack, Typography, IconButton } from "@mui/material";
import SchoolRoundedIcon from "@mui/icons-material/SchoolRounded";
import DarkModeRoundedIcon from "@mui/icons-material/DarkModeRounded";
import LightModeRoundedIcon from "@mui/icons-material/LightModeRounded";
import { useThemeApp } from "../../contexts/ThemeContext";

export default function AuthHeader({ subtitle }: { subtitle: string }) {
  const { mode, toggle } = useThemeApp();

  return (
    <Stack alignItems="center" spacing={1} mb={3} position="relative">
      <IconButton
        onClick={toggle}
        sx={{ position: "absolute", right: 0, top: 0 }}
      >
        {mode === "dark" ? <LightModeRoundedIcon /> : <DarkModeRoundedIcon />}
      </IconButton>

      <Stack direction="row" spacing={1} alignItems="center">
        <SchoolRoundedIcon sx={{ fontSize: 38, color: "primary.main" }} />
        <Typography variant="h4" fontWeight={700}>
          EduConnect
        </Typography>
      </Stack>

      <Typography variant="body2" color="text.secondary" textAlign="center">
        {subtitle}
      </Typography>
    </Stack>
  );
}
