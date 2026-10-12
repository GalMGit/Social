import { apiService } from "../api-service/apiService.ts";
import type { UploadMediaResponse } from "../../models/media/responses/UploadMediaResponse.ts";

export const mediaService = {
    upload: async (file: File) => {
        const formData = new FormData();
        formData.append("file", file);

        const response = await apiService.post<UploadMediaResponse>(
            "media",
            formData
        );

        return response.data;
    },
};
