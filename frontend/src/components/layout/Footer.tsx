import { Box, Typography } from "@mui/material";

export default function Footer() {
  return (
    <Box
      component="footer"
      py={2}
      textAlign="center"
      bgcolor="background.paper"
    >
      <Typography variant="body2">
        © EduConnect 2026 · @educonnect · contato@educonnect.com
      </Typography>
    </Box>
  );
}
