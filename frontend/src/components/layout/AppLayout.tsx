import { Box } from "@mui/material";
import TopBar from "./TopBar";
import Footer from "./Footer";

type Props = {
  children: React.ReactNode;
};

export default function AppLayout({ children }: Props) {
  return (
    <Box minHeight="100vh" display="flex" flexDirection="column">
      <TopBar />

      <Box flex={1} p={4}>
        {children}
      </Box>

      <Footer />
    </Box>
  );
}
