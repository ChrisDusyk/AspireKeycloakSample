import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { AuthProvider, AuthProviderProps } from "react-oidc-context";
import { WebStorageStateStore } from "oidc-client-ts";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import "./index.css";
import App from "./App.tsx";
import { ErrorBoundary } from "react-error-boundary";
import { Fallback } from "./ErrorBoundary.tsx";

const oidcConfig: AuthProviderProps = {
  client_id: "react-client",
  authority: "http://localhost:8080/realms/aspire",
  redirect_uri: "https://localhost:3000",
  post_logout_redirect_uri: "https://localhost:3000",
  response_type: "code",
  scope: "openid profile email weather:all",
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  onSigninCallback: () => {
    // Remove the OIDC state from the URL after signin, otherwise refreshing will
    // cause the auth provider to error.
    window.history.replaceState({}, document.title, window.location.pathname);
  },
};

const reactQueryClient = new QueryClient({
  defaultOptions: {
    queries: {
      throwOnError: true,
    },
  },
});

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ErrorBoundary FallbackComponent={Fallback}>
      <AuthProvider {...oidcConfig}>
        <QueryClientProvider client={reactQueryClient}>
          <App />
        </QueryClientProvider>
      </AuthProvider>
    </ErrorBoundary>
  </StrictMode>
);
