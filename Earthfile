VERSION --wildcard-builds 0.8
IMPORT .config AS build-utils

generate-package.json:
	BUILD ./src/Brain+generate-package.json
    BUILD ./src/Discordia+generate-package.json
    BUILD ./src/Spine+generate-package.json


COPY_PACKAGE_JSON:
	FUNCTION
	COPY ./src/Brain+generate-package.json/package.json ./src/Brain/
    COPY ./src/Discordia+generate-package.json/package.json ./src/Discordia/
    COPY ./src/Spine+generate-package.json/package.json ./src/Spine/

changeset:
	FROM build-utils+changeset-builder
	WORKDIR /repo
	DO +COPY_PACKAGE_JSON
	COPY --dir .changeset ./
	COPY --dir .git ./
	RUN ls -d src/*/ | jq -nR '{ name: "diva", private: true, workspaces: [inputs] }' > package.json
	RUN --interactive npx changeset
	SAVE ARTIFACT .changeset AS LOCAL ./.changeset