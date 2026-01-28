import { Stack, Typography } from "@mui/material";
import SchoolRoundedIcon from "@mui/icons-material/SchoolRounded";

type Props = {
  subtitle: string;
};

export default function AuthHeader({ subtitle }: Props) {
  return (
    <Stack alignItems="center" spacing={1} mb={3}>
      <Stack direction="row" spacing={1} alignItems="center">
        <SchoolRoundedIcon
          sx={{
            fontSize: 38,
            color: "primary.main",
            filter: "drop-shadow(0 0 10px rgba(94,163,255,.6))",
          }}
        />

        <Typography
          variant="h4"
          sx={{
            fontWeight: 700,
            letterSpacing: 1,
          }}
        >
          EduConnect
        </Typography>
      </Stack>

      <Typography
        variant="body2"
        color="text.secondary"
        textAlign="center"
      >
        {subtitle}
      </Typography>
    </Stack>
  );
}